using System.Collections.Generic;
using System.Linq;
using LanguageTranslator.Java.Interfaces;
using LanguageTranslator.Java.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageTranslator.ExtensionPoints.Net
{
    public class ConvertExtensionPoint : IExtensionPoint<InvocationExpressionSyntax>
    {
        public IStmt Translate(InvocationExpressionSyntax expression, SemanticModel semanticModel, CSharpSyntaxVisitor<IStmt> visitor)
        {
            var symbol = GetConvertSymbol(semanticModel, expression);
            if (symbol == null)
                return null;
            var arguments = expression.ArgumentList.Arguments;
            var exactSignatures = new Dictionary<string, string>
            {
                {"ToInt32", "Integer.parseInt"},
                {"ToDouble", "Double.parseDouble"}
            };
            if (exactSignatures.ContainsKey(symbol.Name))
            {
                var dotIndex = exactSignatures[symbol.Name].IndexOf('.');
                var obj = exactSignatures[symbol.Name].Substring(0, dotIndex);
                var method = exactSignatures[symbol.Name].Substring(dotIndex + 1);
                return new CallExpr
                {
                    Expression = new MemberAccessExpr
                    {
                        ObjectExpr = new IdentifierExpr {Identifier = obj},
                        MemberExpr = new IdentifierExpr { Identifier = method }
                    },
                    Arguments = new[] { visitor.Visit(arguments[0]) }
                };
            }
            return null;
        }

        private ISymbol GetConvertSymbol(SemanticModel semanticModel, ExpressionSyntax expression)
        {
            var symbol = semanticModel.GetSymbolInfo(expression).Symbol;
            return symbol != null && symbol.ContainingType.Name == "Convert"
                ? symbol
                : null;
        }        
    }
}
