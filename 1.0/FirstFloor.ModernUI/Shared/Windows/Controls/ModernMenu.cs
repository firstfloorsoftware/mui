using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        public static readonly DependencyProperty LinkGroupsProperty = DependencyProperty.Register("LinkGroups", typeof(LinkGroupCollection), typeof(ModernMenu), new PropertyMetadata(OnLinkGroupsChanged));
        /// <summary>
        /// Defines the SelectedLinkGroup dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedLinkGroupProperty = DependencyProperty.Register("SelectedLinkGroup", typeof(LinkGroup), typeof(ModernMenu), new PropertyMetadata(OnSelectedLinkGroupChanged));
        /// <summary>
        /// Defines the SelectedLink dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedLinkProperty = DependencyProperty.Register("SelectedLink", typeof(Link), typeof(ModernMenu), new PropertyMetadata(OnSelectedLinkChanged));
        /// <summary>
        /// Defines the SelectedSource dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedSourceProperty = DependencyProperty.Register("SelectedSource", typeof(Uri), typeof(ModernMenu), new PropertyMetadata(OnSelectedSourceChanged));

        private static readonly DependencyPropertyKey VisibleLinkGroupsPropertyKey = DependencyProperty.RegisterReadOnly("VisibleLinkGroups", typeof(ReadOnlyLinkGroupCollection), typeof(ModernMenu), null);
        /// <summary>
        /// Defines the VisibleLinkGroups dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleLinkGroupsProperty = VisibleLinkGroupsPropertyKey.DependencyProperty;

        /// <summary>
        /// Occurs when the selected source has changed.
        /// </summary>
        public event EventHandler<SourceEventArgs> SelectedSourceChanged;

        private Dictionary<string, ReadOnlyLinkGroupCollection> groupMap = new Dictionary<string, ReadOnlyLinkGroupCollection>();     // stores LinkGroupCollections by GroupKey
        private bool isSelecting;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernMenu"/> class.
        /// </summary>
        public ModernMenu()
        {
            this.DefaultStyleKey = typeof(ModernMenu);

            // create a default link groups collection
            SetCurrentValue(LinkGroupsProperty, new LinkGroupCollection());
        }

        private static void OnLinkGroupsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ModernMenu)o).OnLinkGroupsChanged((LinkGroupCollection)e.OldValue, (LinkGroupCollection)e.NewValue);
        }

        private void OnLinkGroupsChanged(LinkGroupCollection oldValue, LinkGroupCollection newValue)
        {
            if (oldValue != null) {
                // detach old event handler
                oldValue.CollectionChanged -= OnLinkGroupsCollectionChanged;
            }
            
            if (newValue != null) {
                // ensures the menu is rebuild when changes in the LinkGroups occur
                newValue.CollectionChanged += OnLinkGroupsCollectionChanged;
            }

            RebuildMenu(newValue);
        }

        private static void OnSelectedLinkGroupChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            // retrieve the selected link from the group
            var group = (LinkGroup)e.NewValue;
            Link selectedLink = null;
            if (group != null) {
                selectedLink = group.SelectedLink;

                // if no link selected or link doesn't exist in group links, auto-select first
                if (group.Links != null) {
                    if (selectedLink != null && !group.Links.Any(l => l == selectedLink)) {
                        selectedLink = null;
                    }

                    if (selectedLink == null) {
                        selectedLink = group.Links.FirstOrDefault();
                    }
                }
            }

            // update the selected link
            ((ModernMenu)o).SetCurrentValue(SelectedLinkProperty, selectedLink);
        }

        private static void OnSelectedLinkChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            // update selected source
            var newValue = (Link)e.NewValue;
            Uri selectedSource = null;
            if (newValue != null) {
                selectedSource = newValue.Source;
            }
            ((ModernMenu)o).SetCurrentValue(SelectedSourceProperty, selectedSource);
        }

        private void OnLinkGroupsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RebuildMenu((LinkGroupCollection)sender);
        }

        private static void OnSelectedSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ModernMenu)o).OnSelectedSourceChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        private void OnSelectedSourceChanged(Uri oldValue, Uri newValue) 
        {
            // Uri "Page1.xaml#111" and "Page1#222" points to the same page, but with a different fragment
            // Must remove the fragment to avoid believing we are on different pages.
            Uri oldValueNoFragment = NavigationHelper.RemoveFragment(oldValue);
            Uri newValueNoFragment = NavigationHelper.RemoveFragment(newValue); 

            if (!this.isSelecting) {
                // if old and new are equal, don't do anything
                if (newValueNoFragment != null && newValueNoFragment.Equals(oldValueNoFragment)) {
                    return;
                }

                UpdateSelection();
            }

            // raise SelectedSourceChanged event
            var handler = this.SelectedSourceChanged;
            if (handler != null) {
                handler(this, new SourceEventArgs(newValue));
            }
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
        /// Gets or sets the selected link.
        /// </summary>
        /// <value>The selected link.</value>
        public Link SelectedLink
        {
            get { return (Link)GetValue(SelectedLinkProperty); }
            set { SetValue(SelectedLinkProperty, value); }
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

        /// <summary>
        /// Gets the selected link groups.
        /// </summary>
        public LinkGroup SelectedLinkGroup
        {
            get { return (LinkGroup)GetValue(SelectedLinkGroupProperty); }
        }

        /// <summary>
        /// Gets the collection of link groups that are currently visible.
        /// </summary>
        public ReadOnlyLinkGroupCollection VisibleLinkGroups
        {
            get { return (ReadOnlyLinkGroupCollection)GetValue(VisibleLinkGroupsProperty); }
        }

        /// <summary>
        /// Gets a non-null key for given group.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private static string GetGroupKey(LinkGroup group)
        {
            // use special key for GroupKey <null>
            return group.GroupKey ?? "<null>";
        }

        private void RebuildMenu(LinkGroupCollection groups)
        {
            this.groupMap.Clear();
            if (groups != null) {
                // fill the group map based on group key
                foreach (var group in groups) {
                    var groupKey = GetGroupKey(group);

                    ReadOnlyLinkGroupCollection groupCollection;
                    if (!this.groupMap.TryGetValue(groupKey, out groupCollection)) {
                        // create a new collection for this group key
                        groupCollection = new ReadOnlyLinkGroupCollection(new LinkGroupCollection());
                        this.groupMap.Add(groupKey, groupCollection);
                    }

                    // add the group
                    groupCollection.List.Add(group);
                }
            }

            // update current selection
            UpdateSelection();
        }

        private void UpdateSelection()
        {
            LinkGroup selectedGroup = null;
            Link selectedLink = null;

            Uri sourceNoFragment = NavigationHelper.RemoveFragment(this.SelectedSource);

            if (this.LinkGroups != null) {
                // find the current select group and link based on the selected source
                var linkInfo = (from g in this.LinkGroups
                                from l in g.Links
                                where l.Source == sourceNoFragment
                                select new {
                                    Group = g,
                                    Link = l
                                }).FirstOrDefault();

                if (linkInfo != null) {
                    selectedGroup = linkInfo.Group;
                    selectedLink = linkInfo.Link;
                }
                else {
                    // could not find link and group based on selected source, fall back to selected link group
                    selectedGroup = this.SelectedLinkGroup;

                    // if selected group doesn't exist in available groups, select first group
                    if (!this.LinkGroups.Any(g => g == selectedGroup)) {
                        selectedGroup = this.LinkGroups.FirstOrDefault();
                    }
                }
            }
            
            ReadOnlyLinkGroupCollection groups = null;
            if (selectedGroup != null) {
                // ensure group itself maintains the selected link
                selectedGroup.SelectedLink = selectedLink;

                // find the collection this group belongs to
                var groupKey = GetGroupKey(selectedGroup);
                this.groupMap.TryGetValue(groupKey, out groups);
            }

            this.isSelecting = true;
            // update selection
            SetValue(VisibleLinkGroupsPropertyKey, groups);
            SetCurrentValue(SelectedLinkGroupProperty, selectedGroup);
            SetCurrentValue(SelectedLinkProperty, selectedLink);
            this.isSelecting = false;
        }
    }
}
