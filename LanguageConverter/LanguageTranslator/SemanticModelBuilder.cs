using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LanguageTranslator
{
    public static class SemanticModelBuilder
    {
        public static SemanticModel Build(SyntaxTree csSyntaxTree, IEnumerable<string> assembliesToLoad)
        {
            var dependencyAssemblies = GetDependencyAssemblies(assembliesToLoad);
            var compilation = CSharpCompilation.Create(new Guid().ToString(), new[] { csSyntaxTree }, dependencyAssemblies);
            return compilation.GetSemanticModel(csSyntaxTree);
        }

        private static IEnumerable<MetadataReference> GetDependencyAssemblies(IEnumerable<string> assembliesToLoad)
        {
            var dependencyAssemblies = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Stack<>).Assembly.Location),
            };
            if (assembliesToLoad != null)
                dependencyAssemblies.AddRange(assembliesToLoad.Select(assemblyName => MetadataReference.CreateFromFile(Assembly.LoadFile(assemblyName).Location)));
            return dependencyAssemblies;
        }
    }
}
