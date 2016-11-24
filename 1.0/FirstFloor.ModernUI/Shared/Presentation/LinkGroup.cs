using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FirstFloor.ModernUI.Presentation
{
    /// <summary>
    /// Represents a named group of links.
    /// </summary>
    public class LinkGroup
        : Displayable
    {
        private LinkCollection links = new LinkCollection();

        /// <summary>
        /// The group key property
        /// </summary>
        public static readonly DependencyProperty GroupKeyProperty = DependencyProperty.Register(
            "GroupKey", typeof (string), typeof (LinkGroup), new PropertyMetadata(default(string)));

        /// <summary>
        /// Gets or sets the key of the group.
        /// </summary>
        /// <value>The key of the group.</value>
        /// <remarks>
        /// The group key is used to group link groups in a <see cref="FirstFloor.ModernUI.Windows.Controls.ModernMenu"/>.
        /// </remarks>
        public string GroupKey
        {
            get { return (string) GetValue(GroupKeyProperty); }
            set { SetValue(GroupKeyProperty, value); }
        }

        /// <summary>
        /// The selected link property
        /// </summary>
        public static readonly DependencyProperty SelectedLinkProperty = DependencyProperty.Register(
            "SelectedLink", typeof (Link), typeof (LinkGroup), new PropertyMetadata(default(Link)));

        /// <summary>
        /// Gets or sets the selected link in this group.
        /// </summary>
        /// <value>The selected link.</value>
        internal Link SelectedLink
        {
            get { return (Link) GetValue(SelectedLinkProperty); }
            set { SetValue(SelectedLinkProperty, value); }
        }

        /// <summary>
        /// Gets the links.
        /// </summary>
        /// <value>The links.</value>
        public LinkCollection Links
        {
            get { return this.links; }
        }
    }
}
