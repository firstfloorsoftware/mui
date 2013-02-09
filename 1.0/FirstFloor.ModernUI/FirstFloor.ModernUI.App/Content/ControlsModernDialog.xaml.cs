using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for ControlsModernDialog.xaml
    /// </summary>
    public partial class ControlsModernDialog : UserControl
    {
        public ControlsModernDialog()
        {
            InitializeComponent();
        }

        private void CommonDialog_Click(object sender, RoutedEventArgs e)
        {
            new ModernDialog {
                Title = "Common dialog",
                Content = new LoremIpsum()
            }.ShowDialog();
        }

        private void MessageDialog_Click(object sender, RoutedEventArgs e)
        {
            var result = ModernDialog.ShowMessage("This is a simple Modern UI styled message dialog. Do you like it?", "Message Dialog", MessageBoxButton.YesNo);

            Debug.WriteLine("Dialog result: {0}", result);
        }
    }
}
