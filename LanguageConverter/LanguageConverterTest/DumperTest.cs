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
public interface ITestBase
{
    string Field { get; }        
    int Foo(int x = 2);
}

public abstract class TestBaseClass : ITestBase
{
    private readonly int Field1;
    public string Field { get; }

    public int Foo(int x)
    {
        return x > 0 ? x + 1 : x - 1;
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
