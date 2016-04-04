using LanguageTranslator.CodeGen.Interfaces;
using LanguageTranslator.ExtensionPoints;
using LanguageTranslator.Java;
using LanguageTranslator.Java.Interfaces;
using Microsoft.CodeAnalysis;

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
        }
    }
}
