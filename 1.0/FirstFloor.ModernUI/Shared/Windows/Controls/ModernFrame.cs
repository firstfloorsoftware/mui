using FirstFloor.ModernUI.Windows.Media;
using FirstFloor.ModernUI.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FirstFloor.ModernUI.Windows.Controls
{
    /// <summary>
    /// A simple content frame implementation with navigation support.
    /// </summary>
    public class ModernFrame
        : ContentControl
    {
        /// <summary>
        /// Identifies the KeepAlive attached dependency property.
        /// </summary>
        public static readonly DependencyProperty KeepAliveProperty = DependencyProperty.RegisterAttached("KeepAlive", typeof(bool?), typeof(ModernFrame), new PropertyMetadata(null));
        /// <summary>
        /// Identifies the KeepContentAlive dependency property.
        /// </summary>
        public static readonly DependencyProperty KeepContentAliveProperty = DependencyProperty.Register("KeepContentAlive", typeof(bool), typeof(ModernFrame), new PropertyMetadata(true, OnKeepContentAliveChanged));
        /// <summary>
        /// Identifies the ContentLoader dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentLoaderProperty = DependencyProperty.Register("ContentLoader", typeof(IContentLoader), typeof(ModernFrame), new PropertyMetadata(new DefaultContentLoader(), OnContentLoaderChanged));
        private static readonly DependencyPropertyKey IsLoadingContentPropertyKey = DependencyProperty.RegisterReadOnly("IsLoadingContent", typeof(bool), typeof(ModernFrame), new PropertyMetadata(false));
        /// <summary>
        /// Identifies the IsLoadingContent dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLoadingContentProperty = IsLoadingContentPropertyKey.DependencyProperty;
        /// <summary>
        /// Identifies the Source dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(ModernFrame), new PropertyMetadata(OnSourceChanged));

        /// <summary>
        /// Occurs when navigation to a content fragment begins.
        /// </summary>
        public event EventHandler<FragmentNavigationEventArgs> FragmentNavigation;
        /// <summary>
        /// Occurs when a new navigation is requested.
        /// </summary>
        /// <remarks>
        /// The navigating event is also raised when a parent frame is navigating. This allows for cancelling parent navigation.
        /// </remarks>
        public event EventHandler<NavigatingCancelEventArgs> Navigating;
        /// <summary>
        /// Occurs when navigation to new content has completed.
        /// </summary>
        public event EventHandler<NavigationEventArgs> Navigated;
        /// <summary>
        /// Occurs when navigation has failed.
        /// </summary>
        public event EventHandler<NavigationFailedEventArgs> NavigationFailed;

        private Stack<Uri> history = new Stack<Uri>();
        private Dictionary<Uri, object> contentCache = new Dictionary<Uri, object>();
#if NET4
        private List<WeakReference> childFrames = new List<WeakReference>();        // list of registered frames in sub tree
#else
        private List<WeakReference<ModernFrame>> childFrames = new List<WeakReference<ModernFrame>>();        // list of registered frames in sub tree
#endif
        private CancellationTokenSource tokenSource;
        private bool isNavigatingHistory;
        private bool isResetSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernFrame"/> class.
        /// </summary>
        public ModernFrame()
        {
            this.DefaultStyleKey = typeof(ModernFrame);

            // associate application and navigation commands with this instance
            this.CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseBack, OnBrowseBack, OnCanBrowseBack));
            this.CommandBindings.Add(new CommandBinding(NavigationCommands.GoToPage, OnGoToPage, OnCanGoToPage));
            this.CommandBindings.Add(new CommandBinding(NavigationCommands.Refresh, OnRefresh, OnCanRefresh));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopy, OnCanCopy));

            this.Loaded += OnLoaded;
        }

        private static void OnKeepContentAliveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ModernFrame)o).OnKeepContentAliveChanged((bool)e.NewValue);
        }

        private void OnKeepContentAliveChanged(bool keepAlive)
        {
            // clear content cache
            this.contentCache.Clear();
        }

        private static void OnContentLoaderChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) {
                // null values for content loader not allowed
                throw new ArgumentNullException("ContentLoader");
            }
        }

        private static void OnSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ModernFrame)o).OnSourceChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        private void OnSourceChanged(Uri oldValue, Uri newValue)
        {
            // if resetting source or old source equals new, don't do anything
            if (this.isResetSource || newValue != null && newValue.Equals(oldValue)) {
                return;
            }

            // handle fragment navigation
            string newFragment = null;
            var oldValueNoFragment = NavigationHelper.RemoveFragment(oldValue);
            var newValueNoFragment = NavigationHelper.RemoveFragment(newValue, out newFragment);

            if (newValueNoFragment != null && newValueNoFragment.Equals(oldValueNoFragment)) {
                // fragment navigation
                var args = new FragmentNavigationEventArgs {
                    Fragment = newFragment
                };

                OnFragmentNavigation(this.Content as IContent, args);
            }
            else {
                var navType = this.isNavigatingHistory ? NavigationType.Back : NavigationType.New;

                // only invoke CanNavigate for new navigation
                if (!this.isNavigatingHistory && !CanNavigate(oldValue, newValue, navType)) {
                    return;
                }

                Navigate(oldValue, newValue, navType);
            }
        }

        private bool CanNavigate(Uri oldValue, Uri newValue, NavigationType navigationType)
        {
            var cancelArgs = new NavigatingCancelEventArgs {
                Frame = this,
                Source = newValue,
                IsParentFrameNavigating = true,
                NavigationType = navigationType,
                Cancel = false,
            };
            OnNavigating(this.Content as IContent, cancelArgs);

            // check if navigation cancelled
            if (cancelArgs.Cancel) {
                Debug.WriteLine("Cancelled navigation from '{0}' to '{1}'", oldValue, newValue);

                if (this.Source != oldValue) {
                    // enqueue the operation to reset the source back to the old value
                    Dispatcher.BeginInvoke((Action)(() => {
                        this.isResetSource = true;
                        SetCurrentValue(SourceProperty, oldValue);
                        this.isResetSource = false;
                    }));
                }
                return false;
            }

            return true;
        }

        private void Navigate(Uri oldValue, Uri newValue, NavigationType navigationType)
        {
            Debug.WriteLine("Navigating from '{0}' to '{1}'", oldValue, newValue);

            // set IsLoadingContent state
            SetValue(IsLoadingContentPropertyKey, true);

            // cancel previous load content task (if any)
            // note: no need for thread synchronization, this code always executes on the UI thread
            if (this.tokenSource != null) {
                this.tokenSource.Cancel();
                this.tokenSource = null;
            }

            // push previous source onto the history stack (only for new navigation types)
            if (oldValue != null && navigationType == NavigationType.New) {
                this.history.Push(oldValue);
            }

            object newContent = null;

            if (newValue != null) {
                // content is cached on uri without fragment
                var newValueNoFragment = NavigationHelper.RemoveFragment(newValue);

                if (navigationType == NavigationType.Refresh || !this.contentCache.TryGetValue(newValueNoFragment, out newContent)) {
                    var localTokenSource = new CancellationTokenSource();
                    this.tokenSource = localTokenSource;
                    // load the content (asynchronous!)
                    var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                    var task = this.ContentLoader.LoadContentAsync(newValue, this.tokenSource.Token);

                    task.ContinueWith(t => {
                        try {
                            if (t.IsCanceled || localTokenSource.IsCancellationRequested) {
                                Debug.WriteLine("Cancelled navigation to '{0}'", newValue);
                            }
                            else if (t.IsFaulted) {
                                // raise failed event
                                var failedArgs = new NavigationFailedEventArgs {
                                    Frame = this,
                                    Source = newValue,
                                    Error = t.Exception.InnerException,
                                    Handled = false
                                };

                                OnNavigationFailed(failedArgs);

                                // if not handled, show error as content
                                newContent = failedArgs.Handled ? null : failedArgs.Error;

                                SetContent(newValue, navigationType, newContent, true);
                            }
                            else {
                                newContent = t.Result;
                                if (ShouldKeepContentAlive(newContent)) {
                                    // keep the new content in memory
                                    this.contentCache[newValueNoFragment] = newContent;
                                }

                                SetContent(newValue, navigationType, newContent, false);
                            }
                        }
                        finally {
                            // clear global tokenSource to avoid a Cancel on a disposed object
                            if (this.tokenSource == localTokenSource) {
                                this.tokenSource = null;
                            }

                            // and dispose of the local tokensource
                            localTokenSource.Dispose();
                        }
                    }, scheduler);
                    return;
                }
            }

            // newValue is null or newContent was found in the cache
            SetContent(newValue, navigationType, newContent, false);
        }

        private void SetContent(Uri newSource, NavigationType navigationType, object newContent, bool contentIsError)
        {
            var oldContent = this.Content as IContent;

            // assign content
            this.Content = newContent;

            // do not raise navigated event when error
            if (!contentIsError) {
                var args = new NavigationEventArgs {
                    Frame = this,
                    Source = newSource,
                    Content = newContent,
                    NavigationType = navigationType
                };

                OnNavigated(oldContent, newContent as IContent, args);
            }

            // set IsLoadingContent to false
            SetValue(IsLoadingContentPropertyKey, false);

            if (!contentIsError) {
                // and raise optional fragment navigation events
                string fragment;
                NavigationHelper.RemoveFragment(newSource, out fragment);
                if (fragment != null) {
                    // fragment navigation
                    var fragmentArgs = new FragmentNavigationEventArgs {
                        Fragment = fragment
                    };

                    OnFragmentNavigation(newContent as IContent, fragmentArgs);
                }
            }
        }


        private IEnumerable<ModernFrame> GetChildFrames()
        {
            var refs = this.childFrames.ToArray();
            foreach (var r in refs) {
                var valid = false;
                ModernFrame frame;

#if NET4
                if (r.IsAlive) {
                    frame = (ModernFrame)r.Target;
#else
                if (r.TryGetTarget(out frame)) {
#endif
                    // check if frame is still an actual child (not the case when child is removed, but not yet garbage collected)
                    if (NavigationHelper.FindFrame(null, frame) == this) {
                        valid = true;
                        yield return frame;
                    }
                }

                if (!valid) {
                    this.childFrames.Remove(r);
                }
            }
        }

        private void OnFragmentNavigation(IContent content, FragmentNavigationEventArgs e)
        {
            // invoke optional IContent.OnFragmentNavigation
            if (content != null) {
                content.OnFragmentNavigation(e);
            }

            // raise the FragmentNavigation event
            if (FragmentNavigation != null) {
                FragmentNavigation(this, e);
            }
        }

        private void OnNavigating(IContent content, NavigatingCancelEventArgs e)
        {
            // first invoke child frame navigation events
            foreach (var f in GetChildFrames()) {
                f.OnNavigating(f.Content as IContent, e);
            }

            e.IsParentFrameNavigating = e.Frame != this;

            // invoke IContent.OnNavigating (only if content implements IContent)
            if (content != null) {
                content.OnNavigatingFrom(e);
            }

            // raise the Navigating event
            if (Navigating != null) {
                Navigating(this, e);
            }
        }

        private void OnNavigated(IContent oldContent, IContent newContent, NavigationEventArgs e)
        {
            // invoke IContent.OnNavigatedFrom and OnNavigatedTo
            if (oldContent != null) {
                oldContent.OnNavigatedFrom(e);
            }
            if (newContent != null) {
                newContent.OnNavigatedTo(e);
            }

            // raise the Navigated event
            if (Navigated != null) {
                Navigated(this, e);
            }
        }

        private void OnNavigationFailed(NavigationFailedEventArgs e)
        {
            if (NavigationFailed != null){
                NavigationFailed(this, e);
            }
        }

        /// <summary>
        /// Determines whether the routed event args should be handled.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks>This method prevents parent frames from handling routed commands.</remarks>
        private bool HandleRoutedEvent(CanExecuteRoutedEventArgs args)
        {
            var originalSource = args.OriginalSource as DependencyObject;

            if (originalSource == null) {
                return false;
            }
            return originalSource.AncestorsAndSelf().OfType<ModernFrame>().FirstOrDefault() == this;
        }

        private void OnCanBrowseBack(object sender, CanExecuteRoutedEventArgs e)
        {
            // only enable browse back for source frame, do not bubble
            if (HandleRoutedEvent(e)) {
                e.CanExecute = this.history.Count > 0;
            }
        }

        private void OnCanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            if (HandleRoutedEvent(e)) {
                e.CanExecute = this.Content != null;
            }
        }

        private void OnCanGoToPage(object sender, CanExecuteRoutedEventArgs e)
        {
            if (HandleRoutedEvent(e)) {
                e.CanExecute = e.Parameter is String || e.Parameter is Uri;
            }
        }

        private void OnCanRefresh(object sender, CanExecuteRoutedEventArgs e)
        {
            if (HandleRoutedEvent(e)) {
                e.CanExecute = this.Source != null;
            }
        }

        private void OnBrowseBack(object target, ExecutedRoutedEventArgs e)
        {
            if (this.history.Count > 0) {
                var oldValue = this.Source;
                var newValue = this.history.Peek();     // do not remove just yet, navigation may be cancelled

                if (CanNavigate(oldValue, newValue, NavigationType.Back)) {
                    this.isNavigatingHistory = true;
                    SetCurrentValue(SourceProperty, this.history.Pop());
                    this.isNavigatingHistory = false;
                }
            }
        }

        private void OnGoToPage(object target, ExecutedRoutedEventArgs e)
        {
            var newValue = NavigationHelper.ToUri(e.Parameter);
            SetCurrentValue(SourceProperty, newValue);
        }

        private void OnRefresh(object target, ExecutedRoutedEventArgs e)
        {
            if (CanNavigate(this.Source, this.Source, NavigationType.Refresh)) {
                Navigate(this.Source, this.Source, NavigationType.Refresh);
            }
        }

        private void OnCopy(object target, ExecutedRoutedEventArgs e)
        {
            // copies the string representation of the current content to the clipboard
            Clipboard.SetText(this.Content.ToString());
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var parent = NavigationHelper.FindFrame(NavigationHelper.FrameParent, this);
            if (parent != null) {
                parent.RegisterChildFrame(this);
            }
        }

        private void RegisterChildFrame(ModernFrame frame)
        {
            // do not register existing frame
            if (!GetChildFrames().Contains(frame)) {
#if NET4
                var r = new WeakReference(frame);
#else
                var r = new WeakReference<ModernFrame>(frame);
#endif
                this.childFrames.Add(r);
            }
        }

        /// <summary>
        /// Determines whether the specified content should be kept alive.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private bool ShouldKeepContentAlive(object content)
        {
            var o = content as DependencyObject;
            if (o != null) {
                var result = GetKeepAlive(o);

                // if a value exists for given content, use it
                if (result.HasValue) {
                    return result.Value;
                }
            }
            // otherwise let the ModernFrame decide
            return this.KeepContentAlive;
        }

        /// <summary>
        /// Gets a value indicating whether to keep specified object alive in a ModernFrame instance.
        /// </summary>
        /// <param name="o">The target dependency object.</param>
        /// <returns>Whether to keep the object alive. Null to leave the decision to the ModernFrame.</returns>
        public static bool? GetKeepAlive(DependencyObject o)
        {
            if (o == null) {
                throw new ArgumentNullException("o");
            }
            return (bool?)o.GetValue(KeepAliveProperty);
        }

        /// <summary>
        /// Sets a value indicating whether to keep specified object alive in a ModernFrame instance.
        /// </summary>
        /// <param name="o">The target dependency object.</param>
        /// <param name="value">Whether to keep the object alive. Null to leave the decision to the ModernFrame.</param>
        public static void SetKeepAlive(DependencyObject o, bool? value)
        {
            if (o == null) {
                throw new ArgumentNullException("o");
            }
            o.SetValue(KeepAliveProperty, value);
        }

        /// <summary>
        /// Gets or sets a value whether content should be kept in memory.
        /// </summary>
        public bool KeepContentAlive
        {
            get { return (bool)GetValue(KeepContentAliveProperty); }
            set { SetValue(KeepContentAliveProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content loader.
        /// </summary>
        public IContentLoader ContentLoader
        {
            get { return (IContentLoader)GetValue(ContentLoaderProperty); }
            set { SetValue(ContentLoaderProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is currently loading content.
        /// </summary>
        public bool IsLoadingContent
        {
            get { return (bool)GetValue(IsLoadingContentProperty); }
        }

        /// <summary>
        /// Gets or sets the source of the current content.
        /// </summary>
        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
    }
}
