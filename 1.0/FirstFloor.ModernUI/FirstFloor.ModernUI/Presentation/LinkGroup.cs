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
        private string groupName;
        private Link selectedLink;
        private LinkCollection links = new LinkCollection();

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>The name of the group.</value>
        public string GroupName
        {
            get { return this.groupName; }
            set
            {
                if (this.groupName != value) {
                    this.groupName = value;
                    OnPropertyChanged("GroupName");
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
