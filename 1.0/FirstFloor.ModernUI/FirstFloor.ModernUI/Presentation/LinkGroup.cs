using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstFloor.ModernUI.Presentation
{
    /// <summary>
    /// Represents a named group of links.
    /// </summary>
    public class LinkGroup
        : Displayable
    {
        private string groupKey;
        private Link selectedLink;
        private LinkCollection links = new LinkCollection();

        /// <summary>
        /// Gets or sets the key of the group.
        /// </summary>
        /// <value>The key of the group.</value>
        /// <remarks>
        /// The group key is used to group link groups in a <see cref="FirstFloor.ModernUI.Windows.Controls.ModernMenu"/>.
        /// </remarks>
        public string GroupKey
        {
            get { return this.groupKey; }
            set
            {
                if (this.groupKey != value) {
                    this.groupKey = value;
                    OnPropertyChanged("GroupKey");
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected link in this group.
        /// </summary>
        /// <value>The selected link.</value>
        internal Link SelectedLink
        {
            get { return this.selectedLink; }
            set
            {
                if (this.selectedLink != value) {
                    this.selectedLink = value;
                    OnPropertyChanged("SelectedLink");
                }
            }
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
