using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LanguageTranslator
{
    class SemanticModelBuilder
    {
        public static SemanticModel Build(SyntaxTree csSyntaxTree)
        {
            var compilation = CSharpCompilation.Create("test", new[] { csSyntaxTree });
            return compilation.GetSemanticModel(csSyntaxTree);
        }
    }
}
