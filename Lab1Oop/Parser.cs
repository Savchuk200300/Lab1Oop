using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace SpreadsheetEditor
{
    class Parser
    {
        private readonly string expression;
        private readonly BigInteger powMod = new BigInteger(1000000000000);
        private List<Token> tokens;

        private Dictionary<TokenType, int> tokenTypePriority = new Dictionary<TokenType, int>()
        {
            { TokenType.OpenBracket, 0 },
            { TokenType.CloseBracket, 0 },   
            { TokenType.Add, 2 },
            { TokenType.Subtract, 2 },
            { TokenType.Positive, 3 },
            { TokenType.Negative, 3 },
            { TokenType.Multiply, 4 },
            { TokenType.Divide, 4 },
            { TokenType.Mod, 4 },
            { TokenType.Exp, 5 },
            { TokenType.Number, 10 }
           
        };

        private Dictionary<TokenType, NodeInfo> tokenTypeNodeInfo = new Dictionary<TokenType, NodeInfo>()
        {
            { TokenType.OpenBracket,    NodeInfo.SkipClimb },
            { TokenType.CloseBracket,   NodeInfo.RightAssociative },
            { TokenType.Add,            NodeInfo.NoInfo },
            { TokenType.Subtract,       NodeInfo.NoInfo },
            { TokenType.Positive,       NodeInfo.SkipClimb },
            { TokenType.Negative,       NodeInfo.SkipClimb },
            { TokenType.Multiply,       NodeInfo.NoInfo },
            { TokenType.Divide,         NodeInfo.NoInfo },
            { TokenType.Exp,            NodeInfo.RightAssociative },
            { TokenType.Number,         NodeInfo.NoInfo },
            { TokenType.Mod,            NodeInfo.NoInfo }
        };

        public Parser(string expression)
        {
            this.expression = expression;
        }

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        enum NodeInfo
        {
            NoInfo,
            SkipClimb,
            RightAssociative
        }

        class Node
        {
            public Node(Token token, int priority, NodeInfo info)
            {
                Token = token;
                Priority = priority;
                Info = info;
            }

            public Token Token { get; }
            public int Priority { get; }
            public NodeInfo Info { get; }

            public Node Parent { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
        }

        Node InsertNode(Node current, Node item, NodeInfo info)
        {
            if (info != NodeInfo.SkipClimb)
            {
                if (info == NodeInfo.RightAssociative)
                {
                    while (current.Priority > item.Priority)
                    {
                        current = current.Parent;
                    }
                }
                else
                {
                    while (current.Priority >= item.Priority)
                    {
                        current = current.Parent;
                    }
                }
            }

            if (item.Token.Type == TokenType.CloseBracket)
            {
                Node node = current.Parent;
                node.Right = current.Right;
                if (current.Right != null)
                {
                    current.Right.Parent = node;
                }
                current = node;
            }
            else
            {
                Node node = new Node(item.Token, item.Priority, item.Info);
                node.Right = null;
                node.Left = current.Right;
                if (current.Right != null)
                {
                    current.Right.Parent = node;
                }
                current.Right = node;
                node.Parent = current;
                current = node;
            }

            return current;
        }

        BigInteger EvaluateExpression(Node node)
        {
            if (node == null)
            {
                return new BigInteger(0);
            }

            BigInteger left = EvaluateExpression(node.Left);
            BigInteger right = EvaluateExpression(node.Right);

            switch (node.Token.Type)
            {
                case TokenType.Number:      return node.Token.Value;
                case TokenType.Positive:    return right;
                case TokenType.Negative:    return BigInteger.Negate(right);
                case TokenType.Add:         return BigInteger.Add(left, right);
                case TokenType.Subtract:    return BigInteger.Subtract(left, right);
                case TokenType.Multiply:    return BigInteger.Multiply(left, right);
                case TokenType.Divide:      return BigInteger.Divide(left, right);
                case TokenType.Mod:         return (BigInteger.Subtract(
                                                left, BigInteger.Multiply(right, BigInteger.Divide(left, right))));
                case TokenType.Exp:         return BigInteger.ModPow(left, right, powMod);
                default:                    return new BigInteger(0);
            }
        }

        private string treeMessage;

        public string Jma()
        {
            Node tree = Parse();
            BigInteger result = EvaluateExpression(tree);
            System.Windows.Forms.MessageBox.Show(result.ToString());

            treeMessage = "";
            PrintNode(tree, 4);
            System.Windows.Forms.MessageBox.Show(treeMessage);

            return result.ToString();
        }

        public string Evaluate()
        {
            Node root = ParseTokens();
            BigInteger result = EvaluateExpression(root);
            return result.ToString();
        }

        private void PrintNode(Node node, int indent)
        {
            if (node == null)
            {
                return;
            }

            PrintNode(node.Right, indent + 4);

            string s;
            switch(node.Token.Type)
            {
                case TokenType.Number:      s = node.Token.Value.ToString(); break;
                case TokenType.Positive:    s = "+ve"; break;
                case TokenType.Negative:    s = "-ve"; break;
                case TokenType.Add:         s = "+"; break;
                case TokenType.Multiply:    s = "*"; break;
                case TokenType.Divide:      s = "/"; break;
                case TokenType.Mod:         s = "%"; break;
                case TokenType.Subtract:    s = "-"; break;
                case TokenType.Exp:         s = "^"; break;
                 default:                    s = "error"; break;
            }

            for (int i = 0; i < indent; i ++)
            {
                treeMessage += " ";
            }
            treeMessage += $"{s}\n";

            PrintNode(node.Left, indent + 4);
        }

        Node Parse()
        {
            Lexer tokenizer = new Lexer(expression);
            tokenizer.FindAllTokens();
            tokenizer.PrintTokens();

            Node root = new Node(new Token(TokenType.OpenBracket, "("), 0, NodeInfo.NoInfo);
            Node current = root;

            foreach (Token token in tokenizer.Tokens)
            {
                if (token.Type == TokenType.InputEnd)
                {
                    break;
                }
                Node node = new Node(token, tokenTypePriority[token.Type], tokenTypeNodeInfo[token.Type]);
                current = InsertNode(current, node, node.Info);
            }

            return root.Right;
        }

        private Node ParseTokens()
        {
            Node root = new Node(new Token(TokenType.OpenBracket, "("), 0, NodeInfo.NoInfo);
            Node current = root;

            foreach (Token token in tokens)
            {
                if (token.Type == TokenType.InputEnd)
                {
                    break;
                }
                Node node = new Node(token, tokenTypePriority[token.Type], tokenTypeNodeInfo[token.Type]);
                current = InsertNode(current, node, node.Info);
            }

            return root.Right;
        }
    }
}
