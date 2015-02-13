using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstFloor.ModernUI.Windows.Navigation
{
    /// <summary>
    /// Identifies the types of navigation that are supported.
    /// </summary>
    public enum NavigationType
    {
        /// <summary>
        /// Navigating to new content.
        /// </summary>
        New,
        /// <summary>
        /// Navigating back in the back navigation history.
        /// </summary>
        Back,
        /// <summary>
        /// Reloading the current content.
        /// </summary>
        Refresh
    }
}
