using System.Linq;
using LanguageTranslator.Java;
using LanguageTranslator.Java.Interfaces;
using LanguageTranslator.Java.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageTranslator.ExtensionPoints.Net
{
    public class FuncExtensionPoint : IExtensionPoint<ObjectCreationExpressionSyntax>, IExtensionPoint<InvocationExpressionSyntax>, ITypeResolver
    {
        public string Resolve(ITypeSymbol symbol)
        {
            return symbol.Name == "Func"
                ? "Function"
                : "";
        }

        public IStmt Translate(ObjectCreationExpressionSyntax node, SemanticModel semanticModel, CSharpSyntaxVisitor<IStmt> visitor)
        {
            var type = semanticModel.GetTypeInfo(node).Type;
            if (type == null || type.Name != "Func")
            {
                return null;
            }
            var lambdas = node.DescendantNodes().OfType<SimpleLambdaExpressionSyntax>();
            if (!lambdas.Any())
                return null;
            var lambda = lambdas.First();
            return new BinaryExpr
            {
                Left = visitor.Visit(lambda.Parameter),
                Operation = "->",
                Right = visitor.Visit(lambda.Body)
            };
        }

        public IStmt Translate(InvocationExpressionSyntax node, SemanticModel semanticModel, CSharpSyntaxVisitor<IStmt> visitor)
        {
            var symbol = semanticModel.GetSymbolInfo(node).Symbol;
            if (symbol == null || !SymbolHelper.IsAssignableFrom(symbol, "Func"))
                return null;
            var arguments = node.ArgumentList.Arguments;
            return new CallExpr
            {
                Expression = new MemberAccessExpr
                {
                    ObjectExpr = visitor.Visit(node.Expression),
                    MemberExpr = new IdentifierExpr { Identifier = "apply" }
                },
                Arguments = arguments.Select(visitor.Visit).ToArray()
            };
        }
    }
}
