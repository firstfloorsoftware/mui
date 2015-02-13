using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FirstFloor.ModernUI.Presentation
{
    /// <summary>
    /// Represents a read-only observable collection of link groups.
    /// </summary>
    public class ReadOnlyLinkGroupCollection
        : ReadOnlyObservableCollection<LinkGroup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyLinkGroupCollection"/> class.
        /// </summary>
        /// <param name="list">The <see cref="LinkGroupCollection"/> with which to create this instance of the <see cref="ReadOnlyLinkGroupCollection"/> class.</param>
        public ReadOnlyLinkGroupCollection(LinkGroupCollection list)
            : base(list)
        {
            this.List = list;
        }

        /// <summary>
        /// Provides access to the wrapped list.
        /// </summary>
        internal LinkGroupCollection List { get; private set; }
    }
}
