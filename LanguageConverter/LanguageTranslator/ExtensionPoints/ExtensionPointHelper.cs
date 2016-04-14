using System.Collections.Generic;
using System.Linq;
using LanguageTranslator.Java.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LanguageTranslator.ExtensionPoints
{
    public static class ExtensionPointHelper
    {
        public static IStmt Translate<TNode>(this IEnumerable<IExtensionPoint> extensionPoints, TNode node, SemanticModel semanticModel, CSharpSyntaxVisitor<IStmt> visitor)
        {
            return
                extensionPoints.OfType<IExtensionPoint<TNode>>().Select(extensionPoint => extensionPoint.Translate(node, semanticModel, visitor)).FirstOrDefault(result => result != null);
        }
    }
}
