using System.Collections.Generic;
using System.Linq;
using LanguageTranslator.Java.Interfaces;
using LanguageTranslator.Java.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageTranslator.ExtensionPoints
{
    public static class ExtensionPointHelper
    {
        public static ExpressionSyntax TryGetMemberOwner(SyntaxNode expression)
        {
            var memberAccess = expression as MemberAccessExpressionSyntax;
            if (memberAccess == null)
                return null;
            return memberAccess.Expression;
        }

        public static IStmt Translate<TNode>(this IEnumerable<IExtensionPoint> extensionPoints, TNode node, SemanticModel semanticModel, CSharpSyntaxVisitor<IStmt> visitor)
        {
            return
                extensionPoints.OfType<IExtensionPoint<TNode>>().Select(extensionPoint => extensionPoint.Translate(node, semanticModel, visitor)).FirstOrDefault(result => result != null);
        }

//        public static IStmt TranslateToMemberAccess()
//        {
//            return new CallExpr
//            {
//                Expression = new MemberAccessExpr
//                {
//                    ObjectExpr = visitor.Visit(ExtensionPointHelper.TryGetMemberOwner(expression.Expression)),
//                    MemberExpr = new IdentifierExpr { Identifier = javaMethod }
//                },
//                Arguments = arguments.Select(visitor.Visit).ToArray()
//            };
//        }
    }
}
