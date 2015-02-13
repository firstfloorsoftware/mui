using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstFloor.ModernUI.Windows.Navigation
{
    /// <summary>
    /// Provides data for frame navigation events.
    /// </summary>
    public class NavigationEventArgs
        : NavigationBaseEventArgs
    {
        /// <summary>
        /// Gets a value that indicates the type of navigation that is occurring.
        /// </summary>
        public NavigationType NavigationType { get; internal set; }
        /// <summary>
        /// Gets the content of the target being navigated to.
        /// </summary>
        public object Content { get; internal set; }
    }
}
