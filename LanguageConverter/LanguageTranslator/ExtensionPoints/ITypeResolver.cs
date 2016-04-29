using Microsoft.CodeAnalysis;

namespace LanguageTranslator.ExtensionPoints
{
    public interface ITypeResolver
    {
        string Resolve(ITypeSymbol symbol);
    }
}
