using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        /// Identifies the Source dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(ModernFrame), new PropertyMetadata(OnSourceChanged));
        /// <summary>
        /// Identifies the KeepContentAlive dependency property.
        /// </summary>
        public static readonly DependencyProperty KeepContentAliveProperty = DependencyProperty.Register("KeepContentAlive", typeof(bool), typeof(ModernFrame), new PropertyMetadata(true, OnKeepContentAliveChanged));

        private Stack<Uri> history = new Stack<Uri>();
        private Dictionary<Uri, object> contentCache = new Dictionary<Uri, object>();
        private bool isNavigatingHistory = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernFrame"/> class.
        /// </summary>
        public ModernFrame()
        {
            this.DefaultStyleKey = typeof(ModernFrame);

            // associate navigation commands with this instance
            this.CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseBack, OnBrowseBack, OnCanBrowseBack));
            this.CommandBindings.Add(new CommandBinding(NavigationCommands.GoToPage, OnGoToPage, OnCanGoToPage));
        }

        private static void OnSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ModernFrame)o).OnSourceChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        private void OnSourceChanged(Uri oldValue, Uri newValue)
        {
            Debug.WriteLine("Navigating from '{0}' to '{1}'", oldValue, newValue);

            object newContent = null;

            if (newValue != null) {
                if (!this.contentCache.TryGetValue(newValue, out newContent)) {
                    newContent = Application.LoadComponent(newValue);

                    if (this.KeepContentAlive) {
                        // and store (if keep alive is true)
                        this.contentCache.Add(newValue, newContent);
                    }
                }
            }

            // load content
            this.Content = newContent;

            // push previous page on the history stack
            if (!this.isNavigatingHistory && oldValue != null) {
                this.history.Push(oldValue);
            }
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

        private void OnCanBrowseBack(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.history.Count > 0;
        }

        private void OnCanGoToPage(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is String || e.Parameter is Uri;
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

        /// <summary>
        /// Gets or sets the source of the current content.
        /// </summary>
        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value whether content should be kept in memory.
        /// </summary>
        public bool KeepContentAlive
        {
            get { return (bool)GetValue(KeepContentAliveProperty); }
            set { SetValue(KeepContentAliveProperty, value); }
        }
    }
}
