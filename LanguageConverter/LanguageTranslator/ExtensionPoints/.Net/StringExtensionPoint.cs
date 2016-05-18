using System;
using System.Collections.Generic;
using System.Linq;
using LanguageTranslator.Java.Interfaces;
using LanguageTranslator.Java.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageTranslator.ExtensionPoints.Net
{
    public class StringExtensionPoint : IExtensionPoint<ElementAccessExpressionSyntax>, IExtensionPoint<InvocationExpressionSyntax>, 
                                        IExtensionPoint<MemberAccessExpressionSyntax>, ITypeResolver
    {
        public string Resolve(ITypeSymbol symbol)
        {
            return symbol.Name == "String"
                ? "String"
                : "";
        }

        public IStmt Translate(ElementAccessExpressionSyntax expression, SemanticModel semanticModel, CSharpSyntaxVisitor<IStmt> visitor)
        {
            var symbol = GetStringSymbol(semanticModel, expression);
            if (symbol == null)
                return null;
            return new CallExpr
            {
                Expression = new MemberAccessExpr
                {
                    ObjectExpr = visitor.Visit(expression.Expression),
                    MemberExpr = new IdentifierExpr {Identifier = "charAt"}
                },
                Arguments = new[] {visitor.Visit(expression.ArgumentList.Arguments.First())}
            };
        }

        public IStmt Translate(InvocationExpressionSyntax expression, SemanticModel semanticModel, CSharpSyntaxVisitor<IStmt> visitor)
        {
            var symbol = GetStringSymbol(semanticModel, expression);
            if (symbol == null)
                return null;
            var arguments = expression.ArgumentList.Arguments;
            var exactSignatures = new Dictionary<string, string>
            {
                {"ToLower", "toLowerCase"},
                {"ToUpper", "toUpperCase"},
                {"EndsWith", "endsWith"},
                {"StartsWith", "startsWith"},
                {"IndexOf", "indexOf"},
                {"Substring", "substring"},
                {"Contains", "contains"}
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
            if (symbol.Name == "IsNullOrEmpty") //string.IsNullOrEmpty(s) => s.isEmpty()
            {
                return new CallExpr
                {
                    Expression = new MemberAccessExpr
                    {
                        ObjectExpr = visitor.Visit(arguments.First()),
                        MemberExpr = new IdentifierExpr { Identifier = "isEmpty" }
                    },
                    Arguments = new IStmt[0]
                };
            }            
            return null;
        }

        public IStmt Translate(MemberAccessExpressionSyntax expression, SemanticModel semanticModel, CSharpSyntaxVisitor<IStmt> visitor)
        {
            var symbol = GetStringSymbol(semanticModel, expression);
            if (symbol == null)
                return null;
            var memberToMethodMappings = new Dictionary<string, string>
            {
                {"Length", "length"}
            };
            if (memberToMethodMappings.ContainsKey(symbol.Name))
            {
                var targetMethod = memberToMethodMappings[symbol.Name];
                return new CallExpr
                {
                    Expression = new MemberAccessExpr
                    {
                        ObjectExpr = visitor.Visit(ExtensionPointHelper.TryGetMemberOwner(expression)),
                        MemberExpr = new IdentifierExpr { Identifier = targetMethod }
                    },
                    Arguments = new IStmt[0]
                };
            }
            return null;
        }

        private ISymbol GetStringSymbol(SemanticModel semanticModel, ExpressionSyntax expression)
        {
            var symbol = semanticModel.GetSymbolInfo(expression).Symbol;
            return symbol != null && symbol.ContainingType.Name == "String"
                ? symbol
                : null;
        }        
    }
}
