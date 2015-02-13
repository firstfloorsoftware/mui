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
            var frames = context.AncestorsAndSelf().OfType<ModernFrame>().ToArray();

            if (name == null || name == FrameSelf) {
                // find first ancestor frame
                return frames.FirstOrDefault();
            }
            if (name == FrameParent) {
                // find parent frame
                return frames.Skip(1).FirstOrDefault();
            }
            if (name == FrameTop) {
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

        /// <summary>
        /// Tries to cast specified value to a uri. Either a uri or string input is accepted.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Uri ToUri(object value)
        {
            var uri = value as Uri;
            if (uri == null) {
                var uriString = value as string;
                if (uriString == null || !Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri)) {
                    return null; // no valid uri found
                }
            }
            return uri;
        }

        /// <summary>
        /// Tries to parse a uri with parameters from given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="targetName">Name of the target.</param>
        /// <returns></returns>
        public static bool TryParseUriWithParameters(object value, out Uri uri, out string parameter, out string targetName)
        {
            uri = value as Uri;
            parameter = null;
            targetName = null;

            if (uri == null) {
                var valueString = value as string;
                return TryParseUriWithParameters(valueString, out uri, out parameter, out targetName);
            }
            
            return true;
        }

        /// <summary>
        /// Tries to parse a uri with parameters from given string value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="targetName">Name of the target.</param>
        /// <returns></returns>
        public static bool TryParseUriWithParameters(string value, out Uri uri, out string parameter, out string targetName)
        {
            uri = null;
            parameter = null;
            targetName = null;

            if (value == null) {
                return false;
            }

            // parse uri value for optional parameter and/or target, eg 'cmd://foo|parameter|target'
            string uriString = value;
            var parts = uriString.Split(new char[] { '|' }, 3);
            if (parts.Length == 3) {
                uriString = parts[0];
                parameter = Uri.UnescapeDataString(parts[1]);
                targetName = Uri.UnescapeDataString(parts[2]);
            }
            else if (parts.Length == 2) {
                uriString = parts[0];
                parameter = Uri.UnescapeDataString(parts[1]);
            }

            return Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri);
        }
    }
}
