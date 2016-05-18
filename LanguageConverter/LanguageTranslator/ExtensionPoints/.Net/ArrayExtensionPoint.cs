using System.Collections.Generic;
using LanguageTranslator.Java.Interfaces;
using LanguageTranslator.Java.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageTranslator.ExtensionPoints.Net
{
    public class ArrayExtensionPoint : IExtensionPoint<MemberAccessExpressionSyntax>
    {
        public IStmt Translate(MemberAccessExpressionSyntax expression, SemanticModel semanticModel, CSharpSyntaxVisitor<IStmt> visitor)
        {
            var symbol = GetArraySymbol(semanticModel, expression);
            if (symbol == null)
                return null;
            var mapping = new Dictionary<string, string>
            {
                {"Length", "length"}
            };
            if (mapping.ContainsKey(symbol.Name))
            {
                return new MemberAccessExpr
                {
                    ObjectExpr = visitor.Visit(expression.Expression),
                    MemberExpr = new IdentifierExpr {Identifier = mapping[symbol.Name]}
                };
            }
            return null;
        }

        private static ISymbol GetArraySymbol(SemanticModel semanticModel, ExpressionSyntax expression)
        {
            var symbol = ModelExtensions.GetSymbolInfo(semanticModel, expression).Symbol;
            return symbol != null && symbol.ContainingType.Name == "Array"
                ? symbol
                : null;
        }
    }
}