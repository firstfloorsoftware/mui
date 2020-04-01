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
    /// Interaction logic for ControlsStylesItemsControl.xaml
    /// </summary>
    public partial class ControlsStylesItemsControl : UserControl
    {
        public ControlsStylesItemsControl()
        {
            InitializeComponent();
        }

        private MenuItem CreateSubMenu(string header)
        {
            var item = new MenuItem { Header = header };
            item.Items.Add(new MenuItem { Header = "Item 1" });
            item.Items.Add("Item 2");
            item.Items.Add(new Separator());
            item.Items.Add("Item 3");
            return item;
        }

        private void ShowContextMenu_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = new ContextMenu();
            
            contextMenu.Items.Add(new MenuItem { Header = "Item" });
            contextMenu.Items.Add(new MenuItem { Header = "Item with gesture", InputGestureText="Ctrl+C" });
            contextMenu.Items.Add(new MenuItem { Header = "Item, disabled", IsEnabled = false });
            contextMenu.Items.Add(new MenuItem { Header = "Item, checked", IsChecked = true });
            contextMenu.Items.Add(new MenuItem { Header = "Item, checked and disabled", IsChecked = true, IsEnabled = false });
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(CreateSubMenu("Item with submenu"));

            var menu = CreateSubMenu("Item with submenu, disabled");
            menu.IsEnabled = false;
            contextMenu.Items.Add(menu);
            
            contextMenu.IsOpen = true;
        }
    }
}
