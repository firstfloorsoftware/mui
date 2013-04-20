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

namespace FirstFloor.ModernUI.App.Content
{
    /// <summary>
    /// Interaction logic for ControlsModernWindow.xaml
    /// </summary>
    public partial class ControlsModernWindow : UserControl
    {
        public ControlsModernWindow()
        {
            InitializeComponent();
        }

        private void EmptyWindow_Click(object sender, RoutedEventArgs e)
        {
            // create an empty modern window with lorem content
            // the EmptyWindow ModernWindow styles is found in the mui assembly at Assets/ModernWindowEx.xaml

            var wnd = new ModernWindow {
                Style = (Style)App.Current.Resources["EmptyWindow"],
                Content = new LoremIpsum {
                    Margin = new Thickness(32)
                },
                Width = 480,
                Height = 480
            };

            wnd.Show();
        }
    }
}
