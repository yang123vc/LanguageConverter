using Microsoft.CodeAnalysis;

namespace LanguageTranslator.ExtensionPoints.Net
{
    public class StringExtensionPoint : IExtensionPoint, ITypeResolver
    {
        public string Resolve(ITypeSymbol symbol)
        {
            return symbol.Name == "String"
                ? "String"
                : "";
        }
    }
}
