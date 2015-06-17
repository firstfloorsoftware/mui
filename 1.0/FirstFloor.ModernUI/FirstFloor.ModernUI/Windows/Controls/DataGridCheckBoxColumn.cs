using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FirstFloor.ModernUI.Windows.Controls
{
    /// <summary>
    /// A DataGrid checkbox column using default Modern UI element styles.
    /// </summary>
    public class DataGridCheckBoxColumn
        : System.Windows.Controls.DataGridCheckBoxColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridCheckBoxColumn"/> class.
        /// </summary>
        public DataGridCheckBoxColumn()
        {
            this.ElementStyle = Application.Current.Resources["DataGridCheckBoxStyle"] as Style;
            this.EditingElementStyle = Application.Current.Resources["DataGridEditingCheckBoxStyle"] as Style;
        }
    }
}
