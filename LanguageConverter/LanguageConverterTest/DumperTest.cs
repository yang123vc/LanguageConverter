using System;
using System.IO;
using System.Text;
using LanguageTranslator;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LanguageConverterTest
{
    [TestClass]
    public class DumperTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            const string csCode = "class A { void foo() { var x = new int[] { 1,2,3};}  }";
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(csCode);
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {
                var dumper = new ASTDumper(syntaxTree.GetRoot(), writer);
                dumper.Dump();
            }
            Console.Write(builder.ToString());
        }
    }
}
