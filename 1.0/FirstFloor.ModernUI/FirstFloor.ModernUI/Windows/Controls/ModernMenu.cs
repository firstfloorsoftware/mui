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
    /// Represents the menu in a Modern UI styled window.
    /// </summary>
    public class ModernMenu
        : Control
    {
        /// <summary>
        /// Defines the LinkGroups dependency property.
        /// </summary>
        public static readonly DependencyProperty LinkGroupsProperty = DependencyProperty.Register("LinkGroups", typeof(LinkGroupCollection), typeof(ModernMenu), new PropertyMetadata(new PropertyChangedCallback(OnLinkGroupsChanged)));
        /// <summary>
        /// Defines the SelectedSource dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedSourceProperty = DependencyProperty.Register("SelectedSource", typeof(Uri), typeof(ModernMenu), new PropertyMetadata(new PropertyChangedCallback(OnSelectedSourceChanged)));

        private ListBox mainMenu;
        private ListBox subMenu;
        private bool isSelecting = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernMenu"/> class.
        /// </summary>
        public ModernMenu()
        {
            this.DefaultStyleKey = typeof(ModernMenu);
        }

        private static void OnLinkGroupsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ModernMenu)o).UpdateMenu(true);
        }

        private static void OnSelectedSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ModernMenu)o).UpdateMenu(false);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.mainMenu != null) {
                this.mainMenu.SelectionChanged -= OnMainMenuSelectionChanged;
            }
            if (this.subMenu != null) {
                this.subMenu.SelectionChanged -= OnSubMenuSelectionChanged;
            }

            this.mainMenu = GetTemplateChild("MainMenu") as ListBox;
            this.subMenu = GetTemplateChild("SubMenu") as ListBox;

            if (this.mainMenu != null) {
                this.mainMenu.SelectionChanged += OnMainMenuSelectionChanged;
            }
            if (this.subMenu != null) {
                this.subMenu.SelectionChanged += OnSubMenuSelectionChanged;
            }

            UpdateMenu(true);
        }

        /// <summary>
        /// Gets or sets the link groups.
        /// </summary>
        /// <value>The link groups.</value>
        public LinkGroupCollection LinkGroups
        {
            get { return (LinkGroupCollection)GetValue(LinkGroupsProperty); }
            set { SetValue(LinkGroupsProperty, value); }
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

        private void UpdateMenu(bool forceUpdate)
        {
            if (this.mainMenu == null || this.subMenu == null || this.LinkGroups == null) {
                return;
            }

            var linkInfo = (from linkGroup in this.LinkGroups
                            from link in linkGroup.Links
                            where link.Source == this.SelectedSource
                            select new {
                                LinkGroup = linkGroup,
                                Link = link
                            }).FirstOrDefault();

            if (linkInfo == null) {
                // nothing to do here
                return;
            }

            string selectedGroupName = null;
            var selectedLinkGroup = this.mainMenu.SelectedItem as LinkGroup;
            if (selectedLinkGroup != null) {
                selectedGroupName = selectedLinkGroup.GroupName;
            }

            if (forceUpdate || selectedGroupName != linkInfo.LinkGroup.GroupName || this.mainMenu.Items.Count == 0) {
                this.mainMenu.Items.Clear();
                foreach (var linkGroup in this.LinkGroups.Where(lg => lg.GroupName == linkInfo.LinkGroup.GroupName && lg.Links.Any())) {
                    this.mainMenu.Items.Add(linkGroup);
                }
            }

            linkInfo.LinkGroup.SelectedLink = linkInfo.Link;
            this.mainMenu.SelectedItem = linkInfo.LinkGroup;
            this.subMenu.SelectedItem = linkInfo.Link;
        }

        private void OnMainMenuSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.isSelecting) {
                return;
            }
            this.isSelecting = true;

            // update sub menu
            this.subMenu.Items.Clear();

            var linkGroup = this.mainMenu.SelectedItem as LinkGroup;

            if (linkGroup != null) {
                foreach (var link in linkGroup.Links) {
                    this.subMenu.Items.Add(link);
                }

                // update selected link in sub menu
                var selectedLink = linkGroup.SelectedLink;
                if (selectedLink == null) {
                    // select the first link
                    selectedLink = linkGroup.Links.FirstOrDefault();
                }

                this.isSelecting = false;
                this.subMenu.SelectedItem = selectedLink;
            }
            this.isSelecting = false;
        }

        private void OnSubMenuSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.isSelecting) {
                return;
            }

            var link = this.subMenu.SelectedItem as Link;
            if (link != null) {
                this.SelectedSource = link.Source;
            }
            else {
                this.SelectedSource = null;
            }
        }
    }
}
