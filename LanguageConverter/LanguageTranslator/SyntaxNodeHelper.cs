using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageTranslator
{
    public static class SyntaxNodeHelper
    {
        public static bool InImplicitThisContext(this SyntaxNode node, SemanticModel semanticModel)
        {
            return node != null && InThisContext(node, semanticModel) && !QualifiedByThis(node);
        }

        private static bool QualifiedByThis(this SyntaxNode node)
        {
            var parentNode = node.Parent;
            while (parentNode != null)
            {
                var memberAccess = parentNode as MemberAccessExpressionSyntax;
                if (memberAccess != null && memberAccess.Expression is ThisExpressionSyntax)
                    return true;
                parentNode = parentNode.Parent;
            }
            return false;
        }

        private static bool InThisContext(this SyntaxNode node, SemanticModel semanticModel)
        {
            var symbol = semanticModel.GetSymbolInfo(node).Symbol;
            if (symbol == null)
                return false;
            var kind = symbol.Kind;
            if (kind != SymbolKind.Field && kind != SymbolKind.Method)
                return false;
            return symbol.ContainingType.CanBeReferencedByName;
        }
    }
}
