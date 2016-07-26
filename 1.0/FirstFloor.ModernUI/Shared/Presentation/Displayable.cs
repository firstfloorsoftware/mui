using System.Windows;

namespace FirstFloor.ModernUI.Presentation
{
    /// <summary>
    ///     Provides a base implementation for objects that are displayed in the UI.
    /// </summary>
    public abstract class Displayable
        : NotifyPropertyChanged
    {

        /// <summary>
        ///     DependencyProperty for DisplayName to be able to bind the property
        /// </summary>
        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register("DisplayName", typeof(string), typeof(Displayable), new PropertyMetadata(""));

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set
            {
                SetValue(DisplayNameProperty, value);
            }
        }
    }
}