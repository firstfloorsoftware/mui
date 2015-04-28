using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FirstFloor.ModernUI.Windows.Navigation
{
    /// <summary>
    /// The hyperlink navigator contract.
    /// </summary>
    public interface ILinkNavigator
    {
        /// <summary>
        /// Gets or sets the navigable commands.
        /// </summary>
        CommandDictionary Commands { get; set; }
        /// <summary>
        /// Performs navigation to specified link.
        /// </summary>
        /// <param name="uri">The uri to navigate to.</param>
        /// <param name="source">The source element that triggers the navigation. Required for frame navigation.</param>
        /// <param name="parameter">An optional command parameter or navigation target.</param>
        void Navigate(Uri uri, FrameworkElement source, string parameter = null);
    }
}
