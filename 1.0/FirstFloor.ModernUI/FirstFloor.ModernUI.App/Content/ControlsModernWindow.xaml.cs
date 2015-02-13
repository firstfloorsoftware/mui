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

        private void BlankWindow_Click(object sender, RoutedEventArgs e)
        {
            // create a blank modern window with lorem content
            // the BlankWindow ModernWindow styles is found in the mui assembly at Assets/ModernWindowStyles.xaml

            var wnd = new ModernWindow {
                Style = (Style)App.Current.Resources["BlankWindow"],
                Title = "ModernWindow",
                IsTitleVisible = true == this.title.IsChecked,
                Content = new LoremIpsum(),
                Width = 480,
                Height = 480
            };

            if (true == this.logo.IsChecked) {
                wnd.LogoData = PathGeometry.Parse("F1 M 24.9015,43.0378L 25.0963,43.4298C 26.1685,49.5853 31.5377,54.2651 38,54.2651C 44.4623,54.2651 49.8315,49.5854 50.9037,43.4299L 51.0985,43.0379C 51.0985,40.7643 52.6921,39.2955 54.9656,39.2955C 56.9428,39.2955 58.1863,41.1792 58.5833,43.0379C 57.6384,52.7654 47.9756,61.75 38,61.75C 28.0244,61.75 18.3616,52.7654 17.4167,43.0378C 17.8137,41.1792 19.0572,39.2954 21.0344,39.2954C 23.3079,39.2954 24.9015,40.7643 24.9015,43.0378 Z M 26.7727,20.5833C 29.8731,20.5833 32.3864,23.0966 32.3864,26.197C 32.3864,29.2973 29.8731,31.8106 26.7727,31.8106C 23.6724,31.8106 21.1591,29.2973 21.1591,26.197C 21.1591,23.0966 23.6724,20.5833 26.7727,20.5833 Z M 49.2273,20.5833C 52.3276,20.5833 54.8409,23.0966 54.8409,26.197C 54.8409,29.2973 52.3276,31.8106 49.2273,31.8106C 46.127,31.8106 43.6136,29.2973 43.6136,26.197C 43.6136,23.0966 46.127,20.5833 49.2273,20.5833 Z");
            }
            if (true == this.noresize.IsChecked) {
                wnd.ResizeMode = ResizeMode.NoResize;
            }
            else if (true == this.canminimize.IsChecked) {
                wnd.ResizeMode = ResizeMode.CanMinimize;
            }
            else if (true == this.canresize.IsChecked) {
                wnd.ResizeMode = ResizeMode.CanResize;
            }
            else if (true == this.canresizewithgrip.IsChecked) {
                wnd.ResizeMode = ResizeMode.CanResizeWithGrip;
            }

            wnd.Show();
        }
    }
}
