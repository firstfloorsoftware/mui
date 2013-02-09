using FirstFloor.ModernUI.Windows.Controls.BBCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Navigation;

namespace FirstFloor.ModernUI.Windows.Controls
{
    /// <summary>
    /// A lighweight control for displaying small amounts of rich formatted BBCode content.
    /// </summary>
    public class BBCodeBlock
        : TextBlock
    {
        /// <summary>
        /// Identifies the BBCode dependency property.
        /// </summary>
        public static DependencyProperty BBCodeProperty = DependencyProperty.Register("BBCode", typeof(string), typeof(BBCodeBlock), new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));
        /// <summary>
        /// Identifies the AccentBrush dependency property.
        /// </summary>
        public static DependencyProperty AccentBrushProperty = DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(BBCodeBlock), new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));
        /// <summary>
        /// Initializes a new instance of the <see cref="BBCodeBlock"/> class.
        /// </summary>
        public BBCodeBlock()
        {
            // ensures the implicit BBCodeBlock style is used
            this.DefaultStyleKey = typeof(BBCodeBlock);

            AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(OnRequestNavigate));
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((BBCodeBlock)o).Update();
        }

        private void Update()
        {
            var bbcode = this.BBCode;

            this.Inlines.Clear();

            if (!string.IsNullOrWhiteSpace(bbcode)) {
                Inline inline;
                try {
                    var parser = new BBCodeParser(bbcode) {
                        AccentBrush = this.AccentBrush
                    };
                    inline = parser.Parse();
                }
                catch (Exception) {
                    // parsing failed, display BBCode value as-is
                    inline = new Run { Text = bbcode };
                }
                this.Inlines.Add(inline);
            }
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Help.ShowHelp(null, e.Uri.OriginalString);
        }

        /// <summary>
        /// Gets or sets the BB code.
        /// </summary>
        /// <value>The BB code.</value>
        public string BBCode
        {
            get { return (string)GetValue(BBCodeProperty); }
            set { SetValue(BBCodeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the accent color brush.
        /// </summary>
        /// <value>The accent color brush.</value>
        public Brush AccentBrush
        {
            get { return (Brush)GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }
    }
}
