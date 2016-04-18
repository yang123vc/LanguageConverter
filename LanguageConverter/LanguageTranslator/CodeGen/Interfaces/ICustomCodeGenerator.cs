namespace LanguageTranslator.CodeGen.Interfaces
{
    public interface ICustomCodeGenerator
    {
        string FileExtension { get; }

        string Generate(ICustomSyntaxTree syntaxTree);
    }
}
