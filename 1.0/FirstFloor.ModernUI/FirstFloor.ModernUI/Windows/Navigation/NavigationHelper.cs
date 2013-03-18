using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FirstFloor.ModernUI.Windows.Navigation
{
    /// <summary>
    /// Provides helper function for navigation.
    /// </summary>
    public static class NavigationHelper
    {
        /// <summary>
        /// Identifies the current frame.
        /// </summary>
        public const string FrameSelf = "_self";
        /// <summary>
        /// Identifies the top frame.
        /// </summary>
        public const string FrameTop = "_top";
        /// <summary>
        /// Identifies the parent of the current frame.
        /// </summary>
        public const string FrameParent = "_parent";

        /// <summary>
        /// Finds the frame identified with given name in the specified context.
        /// </summary>
        /// <param name="name">The frame name.</param>
        /// <param name="context">The framework element providing the context for finding a frame.</param>
        /// <returns>The frame or null if the frame could not be found.</returns>
        public static ModernFrame FindFrame(string name, FrameworkElement context)
        {
            if (context == null) {
                throw new ArgumentNullException("context");
            }

            // collect all ancestor frames
            var frames = context.Ancestors().OfType<ModernFrame>().ToArray();

            if (name == null || name == "_self") {
                // find first ancestor frame
                return frames.FirstOrDefault();
            }
            if (name == "_parent") {
                // find parent frame
                return frames.Skip(1).FirstOrDefault();
            }
            if (name == "_top") {
                // find top-most frame
                return frames.LastOrDefault();
            }

            // find ancestor frame having a name matching the target
            var frame = frames.FirstOrDefault(f => f.Name == name);

            if (frame == null) {
                // find frame in context scope
                frame = context.FindName(name) as ModernFrame;

                if (frame == null) {
                    // find frame in scope of ancestor frame content
                    var parent = frames.FirstOrDefault();
                    if (parent != null && parent.Content != null) {
                        var content = parent.Content as FrameworkElement;
                        if (content != null) {
                            frame = content.FindName(name) as ModernFrame;
                        }
                    }
                }
            }

            return frame;
        }

        /// <summary>
        /// Removes the fragment from specified uri and return it.
        /// </summary>
        /// <param name="uri">The uri</param>
        /// <returns>The uri without the fragment, or the uri itself if no fragment is found</returns>
        public static Uri RemoveFragment(Uri uri)
        {
            string fragment;
            return RemoveFragment(uri, out fragment);
        }

        /// <summary>
        /// Removes the fragment from specified uri and returns the uri without the fragment and the fragment itself.
        /// </summary>
        /// <param name="uri">The uri.</param>
        /// <param name="fragment">The fragment, null if no fragment found</param>
        /// <returns>The uri without the fragment, or the uri itself if no fragment is found</returns>
        public static Uri RemoveFragment(Uri uri, out string fragment)
        {
            fragment = null;

            if (uri != null) {
                var value = uri.OriginalString;

                var i = value.IndexOf('#');
                if (i != -1) {
                    fragment = value.Substring(i + 1);
                    uri = new Uri(value.Substring(0, i), uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
                }
            }

            return uri;
        }
    }
}
