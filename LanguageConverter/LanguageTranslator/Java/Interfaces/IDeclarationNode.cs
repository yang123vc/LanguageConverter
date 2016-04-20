using Microsoft.CodeAnalysis;

namespace LanguageTranslator.Java.Interfaces
{
    public interface IDeclarationNode
    {
        DeclarationKind Kind { get; }
        Accessibility DeclaredAccessibility { get; }
    }
}
