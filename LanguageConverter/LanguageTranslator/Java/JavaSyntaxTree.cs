using LanguageTranslator.CodeGen.Interfaces;
using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java
{
    public class JavaSyntaxTree : ICustomSyntaxTree
    {
        public IDeclarationNode[] Declarations { get; set; }
    }
}
