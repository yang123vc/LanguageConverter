using System.Collections.Generic;
using System.Linq;
using LanguageTranslator.CodeGen.Interfaces;
using LanguageTranslator.ExtensionPoints;
using LanguageTranslator.Java;
using LanguageTranslator.Java.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageTranslator
{
    class ModuleTranslator
    {
        private readonly SyntaxTree syntaxTree;
        private readonly SemanticModel semanticModel;
        private readonly IExtensionPoint[] extensionPoints;

        public ModuleTranslator(SyntaxTree syntaxTree, SemanticModel semanticModel, IExtensionPoint[] extensionPoints)
        {
            this.syntaxTree = syntaxTree;
            this.semanticModel = semanticModel;
            this.extensionPoints = extensionPoints;
        }

        public JavaSyntaxTree Translate()
        {
            SyntaxNode rootNode;
            if (!syntaxTree.TryGetRoot(out rootNode))
            {
                return new JavaSyntaxTree
                {
                    Declarations = new IDeclarationNode[0]
                };
            }
            var classTranslator = new ClassTranslator();
            var descedantNodes = rootNode.DescendantNodes();
            var declarations = new List<IDeclarationNode>();
            declarations.AddRange(descedantNodes.OfType<ClassDeclarationSyntax>().Select(decl => classTranslator.Translate(decl)));

            return new JavaSyntaxTree
            {
                Declarations = declarations.ToArray()
            };
        }
    }
}
