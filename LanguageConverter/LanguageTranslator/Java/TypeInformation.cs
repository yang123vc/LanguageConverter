using Microsoft.CodeAnalysis;

namespace LanguageTranslator.Java
{
    public class TypeInformation
    {
        //user types
        public string TypeName { get; set; }
        public ITypeSymbol[] GenericArguments { get; set; }
        //internal types
        public ITypeSymbol TypeSymbol { get; set; }
    }
}
