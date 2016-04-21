using System;
using System.Linq;
using LanguageTranslator.Java.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageTranslator
{
    class InterfaceTranslator
    {
        private readonly SemanticModel semanticModel;

        public InterfaceTranslator(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        public JavaInterface Translate(InterfaceDeclarationSyntax declarationNode, StatementTranslator statementTranslator)
        {
            var symbol = semanticModel.GetDeclaredSymbol(declarationNode);
            if (symbol == null)
                throw new Exception("Cannot build semantic information for interface type");
            var descendantNodes = declarationNode.DescendantNodes().ToArray();
            var methods = descendantNodes.OfType<MethodDeclarationSyntax>()
                                         .Select(method => TranslatorHelper.TranslateMethod(semanticModel, method, statementTranslator)).ToArray();
            var fields = TranslatorHelper.GetFields(declarationNode)
                                         .Select(node => TranslatorHelper.TranslateField(semanticModel, node, statementTranslator));
            var className = declarationNode.Identifier.ToString();
            return new JavaInterface
            {
                Name = className,
                Methods = methods,
                Fields = fields.ToArray(),
                TypeSymbol = symbol,
                DeclaredAccessibility = symbol.DeclaredAccessibility
            };
        }        
    }
}
