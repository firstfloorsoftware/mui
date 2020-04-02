using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace FirstFloor.ModernUI.App.Content
{
    /// <summary>
    /// Interaction logic for ControlsModernButton.xaml
    /// </summary>
    public partial class ControlsModernButton : UserControl
    {
        public ControlsModernButton()
        {
            InitializeComponent();

            // find all embedded XAML icon files
            var assembly = GetType().Assembly;
            var iconResourceNames = from name in assembly.GetManifestResourceNames()
                                    where name.StartsWith("FirstFloor.ModernUI.App.Assets.appbar.")
                                    select name;


            foreach (var name in iconResourceNames) {
                // load the resource stream
                using (var stream = assembly.GetManifestResourceStream(name)) {
                    // parse the icon data using xml
                    var doc = XDocument.Load(stream);

                    var path = doc.Root.Element("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Path");
                    if (path != null) {
                        var data = (string)path.Attribute("Data");

                        // create a modern button and add it to the button panel
                        ButtonPanel.Children.Add(new ModernButton {
                            IconData = PathGeometry.Parse(data),
                            Margin = new Thickness(0, 0, 8, 0)
                        });
                    }
                }
            }
        }
    }
}
