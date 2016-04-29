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
        [TestMethod]
        public void DumpTest()
        {
            const string csCode = @"
public class Prog
{
    public int Foo(int x)
    {
        var str = ""lala"";
        var c = str[1];
    }
}
";
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
