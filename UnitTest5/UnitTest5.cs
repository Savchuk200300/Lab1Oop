using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SpreadsheetEditor;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest5
{
    [TestClass]
    public class UnitTest5
    {
        [TestMethod]
        public void TestMethod1()
        {
            List<Token> tokens;
            string expression = "=2^6";
            Lexer lexer = new Lexer(expression);
            lexer.FindAllTokens();
            tokens = lexer.Tokens;
            Parser parser = new Parser(tokens);
            string ShownValue = parser.Evaluate();
            Assert.AreEqual(ShownValue, "64");
        }
    }
}