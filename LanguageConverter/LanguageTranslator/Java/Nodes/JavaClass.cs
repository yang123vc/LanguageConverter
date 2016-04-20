using LanguageTranslator.Java.Interfaces;
using Microsoft.CodeAnalysis;

namespace LanguageTranslator.Java.Nodes
{
    public class JavaClass : IDeclarationNode
    {
        public string Name { get; set; }
        public JavaField[] Fields { get; set; }
        public IMethod[] Methods { get; set; }
        public DeclarationKind Kind { get { return DeclarationKind.Class; } }
        public Accessibility DeclaredAccessibility { get; set; }
        public INamedTypeSymbol TypeSymbol { get; set; }
    }
}
