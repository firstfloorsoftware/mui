using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using FirstFloor.ModernUI.Windows.Controls;

namespace FirstFloor.ModernUI.Windows.Navigation
{
    /// <summary>
    /// Provides data for the <see cref="IContent.OnNavigatingFrom" /> method and the <see cref="ModernFrame.Navigating"/> event.
    /// </summary>
    public class NavigatingCancelEventArgs
        : NavigationBaseEventArgs
    {
        /// <summary>
        /// Gets a value indicating whether the frame performing the navigation is a parent frame or the frame itself.
        /// </summary>
        public bool IsParentFrameNavigating { get; internal set; }
        /// <summary>
        /// Gets a value that indicates the type of navigation that is occurring.
        /// </summary>
        public NavigationType NavigationType { get; internal set; }
        /// <summary>
        /// Gets or sets a value indicating whether the event should be canceled.
        /// </summary>
        public bool Cancel { get; set; }
    }
}
