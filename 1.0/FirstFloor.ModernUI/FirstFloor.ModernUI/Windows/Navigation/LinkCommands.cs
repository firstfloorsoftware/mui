using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FirstFloor.ModernUI.Windows.Navigation
{
    /// <summary>
    /// The routed link commands.
    /// </summary>
    public static class LinkCommands
    {
        private static RoutedUICommand navigateLink = new RoutedUICommand(Resources.NavigateLink, "NavigateLink", typeof(LinkCommands));

        /// <summary>
        /// Gets the navigate link routed command.
        /// </summary>
        public static RoutedUICommand NavigateLink
        {
            get { return navigateLink; }
        }
    }
}
