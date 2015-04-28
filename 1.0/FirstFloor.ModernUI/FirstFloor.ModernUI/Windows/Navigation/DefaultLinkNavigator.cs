using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Media;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FirstFloor.ModernUI.Windows.Navigation
{
    /// <summary>
    /// The default link navigator with support for loading frame content, external link navigation using the default browser and command execution.
    /// </summary>
    public class DefaultLinkNavigator
        : ILinkNavigator
    {
        private CommandDictionary commands = new CommandDictionary();
        private string[] externalSchemes = new string[] { Uri.UriSchemeHttp, Uri.UriSchemeHttps, Uri.UriSchemeMailto };

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLinkNavigator"/> class.
        /// </summary>
        public DefaultLinkNavigator()
        {
            // register all ApperanceManager commands
            this.Commands.Add(new Uri("cmd://accentcolor"), AppearanceManager.Current.AccentColorCommand);
            this.Commands.Add(new Uri("cmd://darktheme"), AppearanceManager.Current.DarkThemeCommand);
            this.Commands.Add(new Uri("cmd://largefontsize"), AppearanceManager.Current.LargeFontSizeCommand);
            this.Commands.Add(new Uri("cmd://lighttheme"), AppearanceManager.Current.LightThemeCommand);
            this.Commands.Add(new Uri("cmd://settheme"), AppearanceManager.Current.SetThemeCommand);
            this.Commands.Add(new Uri("cmd://smallfontsize"), AppearanceManager.Current.SmallFontSizeCommand);

            // register navigation commands
            this.commands.Add(new Uri("cmd://browseback"), NavigationCommands.BrowseBack);
            this.commands.Add(new Uri("cmd://refresh"), NavigationCommands.Refresh);

            // register application commands
            this.commands.Add(new Uri("cmd://copy"), ApplicationCommands.Copy);
        }

        /// <summary>
        /// Gets or sets the schemes for external link navigation.
        /// </summary>
        /// <remarks>
        /// Default schemes are http, https and mailto.
        /// </remarks>
        public string[] ExternalSchemes
        {
            get { return this.externalSchemes; }
            set { this.externalSchemes = value; }
        }

        /// <summary>
        /// Gets or sets the navigable commands.
        /// </summary>
        public CommandDictionary Commands
        {
            get { return this.commands; }
            set { this.commands = value; }
        }

        /// <summary>
        /// Performs navigation to specified link.
        /// </summary>
        /// <param name="uri">The uri to navigate to.</param>
        /// <param name="source">The source element that triggers the navigation. Required for frame navigation.</param>
        /// <param name="parameter">An optional command parameter or navigation target.</param>
        public virtual void Navigate(Uri uri, FrameworkElement source = null, string parameter = null)
        {
            if (uri == null) {
                throw new ArgumentNullException("uri");
            }

            // first check if uri refers to a command
            ICommand command;
            if (this.commands != null && this.commands.TryGetValue(uri, out command)) {
                // note: not executed within BBCodeBlock context, Hyperlink instance has Command and CommandParameter set
                if (command.CanExecute(parameter)) {
                    command.Execute(parameter);
                }
                else {
                    // do nothing
                }
            }
            else if (uri.IsAbsoluteUri && this.externalSchemes != null && this.externalSchemes.Any(s => uri.Scheme.Equals(s, StringComparison.OrdinalIgnoreCase))) {
                // uri is external, load in default browser
                Process.Start(uri.AbsoluteUri);
                return;
            }
            else {
                // perform frame navigation
                if (source == null) {   // source required
                    throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, ModernUI.Resources.NavigationFailedSourceNotSpecified, uri));
                }

                // use optional parameter as navigation target to identify target frame (_self, _parent, _top or named target frame)
                var frame = NavigationHelper.FindFrame(parameter, source);
                if (frame == null) {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, ModernUI.Resources.NavigationFailedFrameNotFound, uri, parameter));
                }

                // delegate navigation to the frame
                frame.Source = uri;
            }
        }
    }
}
