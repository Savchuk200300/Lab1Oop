using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetEditor
{
    public partial class Form1 : Form
    {
        private const int MAX_ROWS = 30;
        private const int MAX_COLUMNS = 260;

        public int ColumnCount { get { return gridView.Columns.Count; } }
        public int RowCount { get { return gridView.Rows.Count; } }

        private GridCell[,] grid;

        private GridCell SelectedCell
        {
            get
            {
                if (gridView.SelectedCells.Count > 0) return gridView.SelectedCells[0] as GridCell;
                else return null;
            }
        }

        public Form1()
        {
            InitializeComponent();

            LoadGrid(10, 10);
        }

        private string ColumnName(int index)
        {
            string name = "";
            while (index > 0)
            {
                if (index % 26 == 0)
                {
                    name += "Z";
                    index = index / 26 - 1;
                }
                else
                {
                    name += (char)('A' + (index % 26 - 1));
                    index /= 26;
                }
            }
            name.Reverse().ToString();
            return name;
        }

        private void LoadGrid(int height, int width)
        {
            for (int i = 0; i < width; i++)
            {
                AddColumn();
            }

            for (int i = 0; i < height; i ++)
            {
                AddRow();
            }
        }

        private void AddRow()
        {
            if (gridView.Rows.Count >= MAX_ROWS || gridView.Columns.Count == 0)
            {
                return;
            }
            gridView.Rows.Add();
            gridView.Rows[gridView.Rows.Count - 1].HeaderCell.Value = (gridView.Rows.Count).ToString();
        }

        private void AddColumn()
        {
            if (gridView.Columns.Count >= MAX_COLUMNS)
            {
                return;
            }
            string columnName = ((char)('A' + gridView.Columns.Count)).ToString();
            GridColumn column = new GridColumn();
            column.Name = columnName;
            gridView.Columns.Add(column);
        }

        private void RemoveRow()
        {
            if (gridView.Rows.Count > 0)
            {
                gridView.Rows.RemoveAt(gridView.Rows.Count-1);
            }
            UpdateCells();
        }

        private void RemoveColumn()
        {
            if (gridView.Columns.Count > 0)
            {
                gridView.Columns.RemoveAt(gridView.Columns.Count - 1);
            }
            UpdateCells();
        }

        public GridCell GetCell(string adress)
        {
            if (adress.Length < 2)
            {
                return null;
            }

            int column = (int)(adress[0] - 'A');
            int row = int.Parse(adress.Substring(1, adress.Length - 1));

            if (column >= 0 && column < gridView.Columns.Count &&
                row >= 0 && row < gridView.Rows.Count)
            {
                return gridView.Rows[row].Cells[column] as GridCell;
            }
            else
            {
                return null;
            }
        }

        private void UpdateGridArray()
        {
            grid = new GridCell[RowCount, ColumnCount];
            for (int i = 0; i < RowCount; i ++)
            {
                for (int j = 0; j < ColumnCount; j ++)
                {
                    grid[i,j] = gridView.Rows[i].Cells[j] as GridCell;
                }
            }
        }

        private void UpdateCells()
        {
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    (gridView.Rows[i].Cells[j] as GridCell).Updated = false;
                }
            }
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    (gridView.Rows[i].Cells[j] as GridCell).Update();
                }
            }
        }

        private void SaveFile()
        {

        }

        private void LoadFile()
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            RemoveColumn();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RemoveRow();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddColumn();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AddRow();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            GridCell cell = SelectedCell;
            if (cell == null)
            {
                return;
            }

            cell.Expression = textBox1.Text;
            UpdateCells();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (SelectedCell == null)
            {
                return;
            }
            textBox1.Text = SelectedCell.Expression;
        }

       

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
