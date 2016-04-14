using Microsoft.CodeAnalysis;

namespace LanguageTranslator.Java.Interfaces
{
    public interface IMethod : IDeclarationNode
    {
        string Name { get; }
        MethodParameterInfo[] Parameters { get; }
        IStmt Body { get; }
        bool IsStatic { get; }
        IMethodSymbol MethodSymbol { get; }

    }
}
