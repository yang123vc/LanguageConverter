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
    public class ConsoleExtensionPoint : IExtensionPoint<InvocationExpressionSyntax>
    {
        public IStmt Translate(InvocationExpressionSyntax expression, SemanticModel semanticModel, CSharpSyntaxVisitor<IStmt> visitor)
        {
            var symbol = GetConsoleSymbol(semanticModel, expression);
            if (symbol == null)
                return null;
            var arguments = expression.ArgumentList.Arguments;
            if (symbol.Name == "WriteLine")
            {
                return new CallExpr
                {
                    Expression = new MemberAccessExpr
                    {
                        ObjectExpr = new IdentifierExpr {Identifier = "System.out"},
                        MemberExpr = new IdentifierExpr { Identifier = "println" }
                    },
                    Arguments = arguments.Select(visitor.Visit).ToArray()
                };
            }
            if (symbol.Name == "Write")
            {
                return new CallExpr
                {
                    Expression = new MemberAccessExpr
                    {
                        ObjectExpr = new IdentifierExpr { Identifier = "System.out" },
                        MemberExpr = new IdentifierExpr { Identifier = "print" }
                    },
                    Arguments = arguments.Select(visitor.Visit).ToArray()
                };
            }
            if (symbol.Name == "ReadLine")
            {
                return new CallExpr
                {
                    Expression = new MemberAccessExpr
                    {
                        ObjectExpr = new IdentifierExpr { Identifier = "System.console()" },
                        MemberExpr = new IdentifierExpr { Identifier = "readLine" }
                    },
                    Arguments = new IStmt[0]
                };
            }
            return null;
        }

        private ISymbol GetConsoleSymbol(SemanticModel semanticModel, ExpressionSyntax expression)
        {
            var symbol = semanticModel.GetSymbolInfo(expression).Symbol;
            return symbol != null && symbol.ContainingType.Name == "Console"
                ? symbol
                : null;
        }        
    }
}
