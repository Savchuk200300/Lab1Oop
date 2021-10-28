using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SpreadsheetEditor;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest4
{
    [TestClass]
    public class UnitTest4
    {
        [TestMethod]
        public void TestMethod1()
        {
            List<Token> tokens;
            string expression = "=54*45";
            Lexer lexer = new Lexer(expression);
            lexer.FindAllTokens();
            tokens = lexer.Tokens;
            Parser parser = new Parser(tokens);
            string ShownValue = parser.Evaluate();
            Assert.AreEqual(ShownValue, "2430");
        }
    }
}