using System.Collections.Generic;
using System.Linq;
using LanguageTranslator.Java;
using LanguageTranslator.Java.Interfaces;
using LanguageTranslator.Java.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageTranslator.ExtensionPoints.Net
{
    public class StackExtensionPoint : IExtensionPoint<InvocationExpressionSyntax>
    {

        public IStmt Translate(InvocationExpressionSyntax expression, SemanticModel semanticModel, CSharpSyntaxVisitor<IStmt> visitor)
        {
            var symbol = semanticModel.GetSymbolInfo(expression).Symbol;
            if (symbol == null || !SymbolHelper.IsAssignableFrom(symbol, "Stack"))
            {
                return null;
            }
            var arguments = expression.ArgumentList.Arguments;
            var exactSignatures = new Dictionary<string, string>
            {
                {"Push", "push"},
                {"Pop", "pop"},                
            };
            if (exactSignatures.ContainsKey(symbol.Name))
            {
                var javaMethod = exactSignatures[symbol.Name];
                return new CallExpr
                {
                    Expression = new MemberAccessExpr
                    {
                        ObjectExpr = visitor.Visit(ExtensionPointHelper.TryGetMemberOwner(expression.Expression)),
                        MemberExpr = new IdentifierExpr { Identifier = javaMethod }
                    },
                    Arguments = arguments.Select(visitor.Visit).ToArray()
                };
            }
            return null;
        }

        private ISymbol GetStackSymbol(SemanticModel semanticModel, ExpressionSyntax expression)
        {
            var symbol = semanticModel.GetSymbolInfo(expression).Symbol;
            return symbol != null && symbol.ContainingType.Name == "Stack"
                ? symbol
                : null;
        }
    }
}