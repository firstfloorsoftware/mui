using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FirstFloor.ModernUI.Windows.Controls
{
    /// <summary>
    /// Represents a control that contains multiple pages that share the same space on screen.
    /// </summary>
    public class ModernTab
        : Control
    {
        /// <summary>
        /// Identifies the ContentLoader dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentLoaderProperty = DependencyProperty.Register("ContentLoader", typeof(IContentLoader), typeof(ModernTab), new PropertyMetadata(new DefaultContentLoader()));
        /// <summary>
        /// Identifies the Layout dependency property.
        /// </summary>
        public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register("Layout", typeof(TabLayout), typeof(ModernTab), new PropertyMetadata(TabLayout.Tab));
        /// <summary>
        /// Identifies the Links dependency property.
        /// </summary>
        public static readonly DependencyProperty LinksProperty = DependencyProperty.Register("Links", typeof(LinkCollection), typeof(ModernTab), new PropertyMetadata(OnLinksChanged));
        /// <summary>
        /// Identifies the SelectedSource dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedSourceProperty = DependencyProperty.Register("SelectedSource", typeof(Uri), typeof(ModernTab), new PropertyMetadata(OnSelectedSourceChanged));

        /// <summary>
        /// Occurs when the selected source has changed.
        /// </summary>
        public event EventHandler<SourceEventArgs> SelectedSourceChanged;

        private ListBox linkList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernTab"/> control.
        /// </summary>
        public ModernTab()
        {
            this.DefaultStyleKey = typeof(ModernTab);

            // create a default links collection
            SetCurrentValue(LinksProperty, new LinkCollection());
        }

        private static void OnLinksChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ModernTab)o).UpdateSelection();
        }

        private static void OnSelectedSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ModernTab)o).OnSelectedSourceChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        private void OnSelectedSourceChanged(Uri oldValue, Uri newValue)
        {
            UpdateSelection();

            // raise SelectedSourceChanged event
            var handler = this.SelectedSourceChanged;
            if (handler != null) {
                handler(this, new SourceEventArgs(newValue));
            }
        }

        private void UpdateSelection()
        {
            if (this.linkList == null || this.Links == null) {
                return;
            }

            // sync list selection with current source
            this.linkList.SelectedItem = this.Links.FirstOrDefault(l => l.Source == this.SelectedSource);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.linkList != null) {
                this.linkList.SelectionChanged -= OnLinkListSelectionChanged;
            }

            this.linkList = GetTemplateChild("LinkList") as ListBox;
            if (this.linkList != null) {
                this.linkList.SelectionChanged += OnLinkListSelectionChanged;
            }

            UpdateSelection();
        }

        private void OnLinkListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var link = this.linkList.SelectedItem as Link;
            if (link != null && link.Source != this.SelectedSource) {
                SetCurrentValue(SelectedSourceProperty, link.Source);
            }
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
        /// Gets or sets a value indicating how the tab should be rendered.
        /// </summary>
        public TabLayout Layout
        {
            get { return (TabLayout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        /// <summary>
        /// Gets or sets the collection of links that define the available content in this tab.
        /// </summary>
        public LinkCollection Links
        {
            get { return (LinkCollection)GetValue(LinksProperty); }
            set { SetValue(LinksProperty, value); }
        }

        /// <summary>
        /// Gets or sets the source URI of the selected link.
        /// </summary>
        /// <value>The source URI of the selected link.</value>
        public Uri SelectedSource
        {
            get { return (Uri)GetValue(SelectedSourceProperty); }
            set { SetValue(SelectedSourceProperty, value); }
        }
    }
}
