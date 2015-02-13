using FirstFloor.ModernUI.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstFloor.ModernUI.Windows
{
    /// <summary>
    /// Defines the optional contract for content loaded in a ModernFrame.
    /// </summary>
    public interface IContent
    {
        /// <summary>
        /// Called when navigation to a content fragment begins.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        void OnFragmentNavigation(FragmentNavigationEventArgs e);
        /// <summary>
        /// Called when this instance is no longer the active content in a frame.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        void OnNavigatedFrom(NavigationEventArgs e);
        /// <summary>
        /// Called when a this instance becomes the active content in a frame.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        void OnNavigatedTo(NavigationEventArgs e);
        /// <summary>
        /// Called just before this instance is no longer the active content in a frame.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        /// <remarks>The method is also invoked when parent frames are about to navigate.</remarks>
        void OnNavigatingFrom(NavigatingCancelEventArgs e);
    }
}
