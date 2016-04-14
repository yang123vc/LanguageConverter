using System;
using System.IO;
using System.Text;
using LanguageTranslator;
using LanguageTranslator.Java;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LanguageConverterTest
{
    [TestClass]
    public class DumperTest
    {
//        public int Field0, Field1;
//        public string Field2;
        [TestMethod]
        public void DumpTest()
        {
            const string csCode = "class A() { public int Field0, Field1; public string Field2; public int main(){ return 0; }";
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
