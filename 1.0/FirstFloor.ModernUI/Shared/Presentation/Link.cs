using System;
using System.Windows;

namespace FirstFloor.ModernUI.Presentation
{
    /// <summary>
    ///     Represents a displayable link.
    /// </summary>
    public class Link
        : Displayable
    {
        /// <summary>
        ///     DependencyProperty for Source to be able to bind the value
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(Link), new PropertyMetadata(null));

        /// <summary>
        ///     Gets or sets the source uri.
        /// </summary>
        /// <value>The source.</value>
        public Uri Source
        {
            get { return (Uri) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
    }
}