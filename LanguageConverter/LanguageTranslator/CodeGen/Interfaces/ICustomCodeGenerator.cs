namespace LanguageTranslator.CodeGen.Interfaces
{
    interface ICustomCodeGenerator
    {
        string FileExtension { get; }

        string Generate(ICustomSyntaxTree syntaxTree);
    }
}
