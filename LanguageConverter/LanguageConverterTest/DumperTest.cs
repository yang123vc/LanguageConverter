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
public class AlgorithmState
{
    public int PositionInText { get; private set; }
    public int GetCriticalFactorizationPosition(int prefixLength)
    {
        //comment
        if (prefixLength <= 1) 
            throw new InvalidUsageException(@""Prefix length must be greater than 1"");
        return 0;
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
