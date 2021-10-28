using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEditor
{
    public enum TokenType
    {
        WhiteSpace,
        BadToken,
        InputEnd,
        Number,
        Cell,
        OpenBracket,
        CloseBracket,
        Add,
        Subtract,
        Multiply,
        Divide,
        Positive,
        Negative,
        Exp,
        Mod
    }

    class Token
    {
        public Token(TokenType type, string text)
        {
            Type = type;
            Text = text;

            if (type == TokenType.Number)
            {
                value = new BigInteger(0);
                BigInteger.TryParse(text, out value);
            }
        }

        private BigInteger value;
        public BigInteger Value
        {
            get
            {
                if (Type == TokenType.Number) return value;
                else return new BigInteger(0);
            }
        }

        public TokenType Type { get; }
        public string Text { get; }

    }
}
