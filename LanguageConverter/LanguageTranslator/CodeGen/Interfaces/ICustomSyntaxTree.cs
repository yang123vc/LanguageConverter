using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.CodeGen.Interfaces
{
    public interface ICustomSyntaxTree
    {
        IDeclarationNode[] Declarations { get; }
    }
}
