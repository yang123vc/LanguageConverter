using System.IO;
using Microsoft.CodeAnalysis;

namespace LanguageTranslator
{
    public class ASTDumper
    {
        private readonly SyntaxNode rootNode;
        private readonly TextWriter outputWriter;

        public ASTDumper(SyntaxNode rootNode, TextWriter outputWriter)
        {
            this.rootNode = rootNode;
            this.outputWriter = outputWriter;
        }

        public void Dump()
        {
            Dump(rootNode, 0);
        }

        private void Dump(SyntaxNode node, int deep)
        {
            var spaces = new string(' ', deep * 2);
            outputWriter.Write(spaces);
            outputWriter.WriteLine(node.GetType().Name);
            foreach (var childNode in node.ChildNodes())
            {
                Dump(childNode, deep + 1);
            }
        }
    }
}
