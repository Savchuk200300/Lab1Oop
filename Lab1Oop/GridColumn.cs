using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetEditor
{
    public class GridColumn : DataGridViewColumn
    {
        public GridColumn()
        {
            this.CellTemplate = new GridCell();
        }
    }
}
