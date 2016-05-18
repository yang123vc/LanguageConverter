using System.Collections.Generic;
using System.Linq;
using LanguageTranslator.Java.Interfaces;
using LanguageTranslator.Java.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageTranslator.ExtensionPoints.Net
{
    public class ObjectExtensionPoint : IExtensionPoint<InvocationExpressionSyntax>
    {
        public IStmt Translate(InvocationExpressionSyntax expression, SemanticModel semanticModel, CSharpSyntaxVisitor<IStmt> visitor)
        {
            var symbol = ModelExtensions.GetSymbolInfo(semanticModel, expression).Symbol;
            if (symbol == null)
                return null;
            if (symbol.Name == "ToString")
            {
                return new CallExpr
                {
                    Expression = new MemberAccessExpr
                    {
                        ObjectExpr = new IdentifierExpr {Identifier = "String"},
                        MemberExpr = new IdentifierExpr { Identifier = "valueOf" }
                    },
                    Arguments = new [] { visitor.Visit(ExtensionPointHelper.TryGetMemberOwner(expression.Expression)) }
                };
            }
            return null;
        }

    }
}