using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEditor
{
    class Lexer
    {
        private readonly string expression; 
        private int position;

        public Lexer(string expression)
        {
            this.expression = expression;
        }

        private enum CommaToken { Min, Max }
        private Stack<CommaToken> commaTokens = new Stack<CommaToken>();

        private List<Token> tokens = new List<Token>();
        public List<Token> Tokens { get { return tokens; } }

        private char Current
        {
            get
            {
                if (position >= expression.Length) return '\0';
                return expression[position];
            }
        }

        private Token LastToken
        {
            get
            {
                if (tokens.Count == 0) return null;
                return tokens[tokens.Count - 1];
            }
        }

        public void FindAllTokens()
        {
            while (true)
            {
                NextStep();
                if (LastToken?.Type == TokenType.InputEnd)
                {
                    break;
                }
            }
        }

        public void PrintTokens()
        {
            string message = "";

            foreach (Token token in tokens)
            {
                message += $"{token.Type.ToString()} {token.Text}\n";
            }

            System.Windows.Forms.MessageBox.Show(message);
        }

        private void NextChar(int count = 1)
        {
            position += count;
        }

        private void NextStep()
        {
            if (position >= expression.Length)
            {
                tokens.Add(new Token(TokenType.InputEnd, null));
                return;
            }

            if (char.IsWhiteSpace(Current))
            {
                while (char.IsWhiteSpace(Current))
                {
                    NextChar();
                }
                return;
            }

            if (char.IsDigit(Current))
            {
                int start = position;
                while (char.IsDigit(Current))
                {
                    NextChar();
                }
                int length = position - start;
                string text = expression.Substring(start, length);
                tokens.Add(new Token(TokenType.Number, text));
                return;
            }

            if (char.IsUpper(Current))
            {
                int start = position;
                NextChar();
                while (char.IsDigit(Current))
                {
                    NextChar();
                }
                int length = position - start;
                string text = expression.Substring(start, length);
                tokens.Add(new Token(TokenType.Cell, text));
                return;
            }

            if (Current == '(')
            {
                tokens.Add(new Token(TokenType.OpenBracket, "("));
                NextChar();
                return;
            }

            if (Current == ')')
            {
                tokens.Add(new Token(TokenType.CloseBracket, ")"));
                NextChar();
                return;
            }

            if (Current == '+')
            {
                if (LastToken?.Type == TokenType.Cell || LastToken?.Type == TokenType.Number || 
                    LastToken?.Type == TokenType.CloseBracket)
                {
                    tokens.Add(new Token(TokenType.Add, "+"));
                }
                else
                {
                    tokens.Add(new Token(TokenType.Positive, "+"));
                }
                NextChar();
                return;
            }

            if (Current == '-')
            {
                if (LastToken?.Type == TokenType.Cell || LastToken?.Type == TokenType.Number || 
                    LastToken?.Type == TokenType.CloseBracket)
                {
                    tokens.Add(new Token(TokenType.Subtract, "-"));
                }
                else
                {
                    tokens.Add(new Token(TokenType.Negative, "-"));
                }
                NextChar();
                return;
            }

            if (Current == '^')
            {
                tokens.Add(new Token(TokenType.Exp, "^"));
                NextChar();
                return;
            }

            if (Current == '*')
            {
                tokens.Add(new Token(TokenType.Multiply, "*"));
                NextChar();
                return;
            }

            if (Current == '/')
            {
                tokens.Add(new Token(TokenType.Divide, "/"));
                NextChar();
                return;
            }

            if (Current == '%')
            {
                tokens.Add(new Token(TokenType.Mod, "%"));
                NextChar();
                return;
            }

          

            NextChar();
            return;
        }
    }
}
