using LanguageTranslator.Java.Nodes;
using Microsoft.CodeAnalysis;

namespace LanguageTranslator.Java.Interfaces
{
    public interface IClassOrInterface : IDeclarationNode
    {
        string Name { get; }
        JavaField[] Fields { get; }
        IMethod[] Methods { get; }
        INamedTypeSymbol TypeSymbol { get; }
        bool IsAbstract { get; }
    }
}
