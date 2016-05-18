using LanguageTranslator.Java.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LanguageTranslator.ExtensionPoints
{
    public interface IExtensionPoint
    {
    }

    public interface IExtensionPoint<in TNode> : IExtensionPoint
    {
        IStmt Translate(TNode expression, SemanticModel semanticModel, CSharpSyntaxVisitor<IStmt> visitor);
    }
}
