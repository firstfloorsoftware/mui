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

        private Stack<Uri> history = new Stack<Uri>();
        private Dictionary<Uri, object> contentCache = new Dictionary<Uri, object>();
        private bool isNavigatingHistory = false;
        private CancellationTokenSource tokenSource;

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
            LoadContent(oldValue, newValue, false);
        }

        private async void LoadContent(Uri oldValue, Uri newValue, bool noCache) 
        {
            Debug.WriteLine("Navigating from '{0}' to '{1}'", oldValue, newValue);

            object newContent = null;

            // set IsLoadingContent state
            SetValue(IsLoadingContentPropertyKey, true);

            // cancel previous load content task (if any)
            // note: no need for thread synchronization, this code always executes on the UI thread
            if (this.tokenSource != null) {
                this.tokenSource.Cancel();
                this.tokenSource = null;
            }

            // push previous source onto the history stack
            if (!this.isNavigatingHistory && oldValue != null) {
                this.history.Push(oldValue);
            }

            if (newValue != null) {
                var contentIsError = false;

                if (noCache || !this.contentCache.TryGetValue(newValue, out newContent)) {
                    using (var localTokenSource = new CancellationTokenSource()) {
                        this.tokenSource = localTokenSource;
                        try {
                            // load the content (asynchronous!)
                            newContent = await this.ContentLoader.LoadContentAsync(newValue, this.tokenSource.Token);

                            // check if task has been cancelled (happens when ContentLoader ignores the CancellationTokenSource and does not throw TaskCancelledException)
                            if (localTokenSource.IsCancellationRequested) {
                                Debug.WriteLine("Cancelled navigation to '{0}'", newValue);
                                return;
                            }
                        }
                        catch (TaskCanceledException) {
                            // load content task has been cancelled, log it and quit
                            Debug.WriteLine("Cancelled navigation to '{0}'", newValue);
                            return;
                        }
                        catch (Exception e) {
                            newContent = e;     // the new content is the exception
                            contentIsError = true;
                        }
                        finally {
                            // clear global tokenSource to avoid a Cancel on a disposed object
                            if (this.tokenSource == localTokenSource) {
                                this.tokenSource = null;
                            }
                        }
                    }
                }

                if (!contentIsError && ShouldKeepContentAlive(newContent)) {
                    // keep the new content in memory
                    this.contentCache[newValue] = newContent;
                }
            }

            // assign content
            this.Content = newContent;

            // set IsLoadingContent to false
            SetValue(IsLoadingContentPropertyKey, false);
        }

        private void OnCanBrowseBack(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.history.Count > 0;
        }

        private void OnCanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.Content != null;
        }

        private void OnCanGoToPage(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is String || e.Parameter is Uri;
        }

        private void OnCanRefresh(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.Source != null;
        }

        private void OnBrowseBack(object target, ExecutedRoutedEventArgs e)
        {
            if (this.history.Count > 0) {
                this.isNavigatingHistory = true;
                this.Source = this.history.Pop();
                this.isNavigatingHistory = false;
            }
        }

        private void OnGoToPage(object target, ExecutedRoutedEventArgs e)
        {
            var source = e.Parameter as Uri;
            if (source != null) {
                this.Source = source;
            }
            else {
                var sourceStr = e.Parameter as string;
                if (sourceStr != null) {
                    this.Source = new Uri(sourceStr, UriKind.RelativeOrAbsolute);
                }
            }
        }

        private void OnRefresh(object target, ExecutedRoutedEventArgs e)
        {
            LoadContent(null, this.Source, true);
        }

        private void OnCopy(object target, ExecutedRoutedEventArgs e)
        {
            // copies the string representation of the current content to the clipboard
            Clipboard.SetText(this.Content.ToString());
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
