using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetEditor
{
    public class GridCell : DataGridViewTextBoxCell
    {
        private string expression;
        private string shownValue;
        private List<GridCell> referencedCells = new List<GridCell>();
        private List<Token> tokens;
        
        public bool Updated { get; set; }
        public string ShownValue { get; set; }

        public string Expression 
        {
            get { return expression; }
            set { expression = value; Update(); } 
        }

        public GridCell()
        {
            Updated = true;
        }

        private GridCell GetCell(string adress)
        {
            if (adress.Length < 2)
            {
                return null;
            }

            DataGridView gridView = DataGridView;

            int column = (int)(adress[0] - 'A');
            int row = int.Parse(adress.Substring(1, adress.Length - 1))-1;

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

        public void Update()
        {
            if (Updated)
            {
                return;
            }
            if (expression == null)
            {
                Updated = true;
                return;
            }

            Lexer lexer = new Lexer(expression);
            lexer.FindAllTokens();
            tokens = lexer.Tokens;
            
            referencedCells.Clear();
            foreach (Token token in tokens)
            {
                if (token.Type == TokenType.Cell)
                {
                    GridCell cell = GetCell(token.Text);
                    if (cell != null)
                    {
                        cell.Update();
                    }
                }
            }

            for (int i = 0; i < tokens.Count; i ++)
            {
                if (tokens[i].Type == TokenType.Cell)
                {
                    GridCell cell = GetCell(tokens[i].Text);
                    if (cell != null)
                    {
                        cell.Update();
                        tokens[i] = new Token(TokenType.Number, cell.ShownValue);
                    }
                }
            }

            Parser parser = new Parser(tokens);
            ShownValue = parser.Evaluate();
            Value = ShownValue;
            Updated = true;
        }
    }
}
