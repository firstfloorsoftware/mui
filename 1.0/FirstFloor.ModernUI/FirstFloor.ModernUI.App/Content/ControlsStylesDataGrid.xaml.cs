using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    // taken from MSDN (http://msdn.microsoft.com/en-us/library/system.windows.controls.datagrid.aspx)
    public enum OrderStatus { None, New, Processing, Shipped, Received };
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsMember { get; set; }
        public OrderStatus Status { get; set; }
    }


    /// <summary>
    /// Interaction logic for ControlsStylesDataGrid.xaml
    /// </summary>
    public partial class ControlsStylesDataGrid : UserControl
    {
        public ControlsStylesDataGrid()
        {
            InitializeComponent();

            ObservableCollection<Customer> custdata = GetData();

            //Bind the DataGrid to the customer data
            DG1.DataContext = custdata;
        }

        private ObservableCollection<Customer> GetData()
        {
            var customers = new ObservableCollection<Customer>();
            customers.Add(new Customer { FirstName = "Orlando", LastName = "Gee", Email = "orlando0@adventure-works.com", IsMember = true, Status = OrderStatus.New });
            customers.Add(new Customer { FirstName = "Keith", LastName = "Harris", Email = "keith0@adventure-works.com", IsMember = true, Status = OrderStatus.Received });
            customers.Add(new Customer { FirstName = "Donna", LastName = "Carreras", Email = "donna0@adventure-works.com", IsMember = false, Status = OrderStatus.None });
            customers.Add(new Customer { FirstName = "Janet", LastName = "Gates", Email = "janet0@adventure-works.com", IsMember = true, Status = OrderStatus.Shipped });
            customers.Add(new Customer { FirstName = "Lucy", LastName = "Harrington", Email = "lucy0@adventure-works.com", IsMember = false, Status = OrderStatus.New });
            customers.Add(new Customer { FirstName = "Rosmarie", LastName = "Carroll", Email = "rosmarie0@adventure-works.com", IsMember = true, Status = OrderStatus.Processing });
            customers.Add(new Customer { FirstName = "Dominic", LastName = "Gash", Email = "dominic0@adventure-works.com", IsMember = true, Status = OrderStatus.Received });
            customers.Add(new Customer { FirstName = "Kathleen", LastName = "Garza", Email = "kathleen0@adventure-works.com", IsMember = false, Status = OrderStatus.None });
            customers.Add(new Customer { FirstName = "Katherine", LastName = "Harding", Email = "katherine0@adventure-works.com", IsMember = true, Status = OrderStatus.Shipped });
            customers.Add(new Customer { FirstName = "Johnny", LastName = "Caprio", Email = "johnny0@adventure-works.com", IsMember = false, Status = OrderStatus.Processing });

            return customers;
        }
    }
}
