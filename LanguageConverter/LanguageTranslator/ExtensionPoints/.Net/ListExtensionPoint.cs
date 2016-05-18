using System.Linq;
using LanguageTranslator.Java;
using LanguageTranslator.Java.Interfaces;
using LanguageTranslator.Java.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageTranslator.ExtensionPoints.Net
{
    public class ListExtensionPoint : IExtensionPoint<ElementAccessExpressionSyntax>, IExtensionPoint<ObjectCreationExpressionSyntax>, ITypeResolver
    {

        public IStmt Translate(ElementAccessExpressionSyntax node, SemanticModel semanticModel, CSharpSyntaxVisitor<IStmt> visitor)
        {
            var symbol = ModelExtensions.GetSymbolInfo(semanticModel, node).Symbol;
            if (symbol == null || !SymbolHelper.IsAssignableFrom(symbol, "IList"))
            {
                return null;
            }
            return new CallExpr
            {
                Expression = new MemberAccessExpr
                {
                    ObjectExpr = visitor.Visit(node.Expression),
                    MemberExpr = new IdentifierExpr { Identifier = "get" }
                },
                Arguments = new[] { visitor.Visit(node.ArgumentList.Arguments.First()) }
            };
        }

        public IStmt Translate(ObjectCreationExpressionSyntax node, SemanticModel semanticModel, CSharpSyntaxVisitor<IStmt> visitor)
        {
            var symbol = ModelExtensions.GetSymbolInfo(semanticModel, node).Symbol;
            if (symbol == null || !SymbolHelper.IsAssignableFrom(symbol, "IList"))
            {
                return null;
            }
            return new ObjectCreationExpr
            {
                TypeInformation = new TypeInformation
                {
                    TypeSymbol = symbol.ContainingType,
                    TypeName = "ArrayList"
                },
                Arguments = node.ArgumentList.Arguments.Select(visitor.Visit).ToArray()
            };
        }

        public string Resolve(ITypeSymbol symbol)
        {
            return SymbolHelper.IsAssignableFrom(symbol, "List")
                ? "ArrayList"
                : "";
        }
    }
}