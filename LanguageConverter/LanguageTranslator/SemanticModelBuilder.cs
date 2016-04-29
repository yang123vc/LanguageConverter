using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LanguageTranslator
{
    public static class SemanticModelBuilder
    {
        public static SemanticModel Build(SyntaxTree csSyntaxTree)
        {
            var dependencyAssemblies = GetDependencyAssemblies();
            var compilation = CSharpCompilation.Create("test", new[] { csSyntaxTree }, dependencyAssemblies);
            return compilation.GetSemanticModel(csSyntaxTree);
        }

        private static IEnumerable<MetadataReference> GetDependencyAssemblies()
        {
            var dependencyAssemblies = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            };            
            return dependencyAssemblies;
        }
    }
}
