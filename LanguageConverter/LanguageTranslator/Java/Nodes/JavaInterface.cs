using LanguageTranslator.Java.Interfaces;
using Microsoft.CodeAnalysis;

namespace LanguageTranslator.Java.Nodes
{
    class JavaInterface : IClassOrInterface
    {
        public string Name { get; set; }
        public JavaField[] Fields { get; set; }
        public IMethod[] Methods { get; set; }
        public DeclarationKind Kind { get { return DeclarationKind.Interface; } }
        public Accessibility DeclaredAccessibility { get; set; }
        public INamedTypeSymbol TypeSymbol { get; set; }
        public bool IsAbstract { get; set; }
    }
}
