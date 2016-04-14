using Microsoft.CodeAnalysis;

namespace LanguageTranslator.Java
{
    public class MethodParameterInfo
    {
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public IParameterSymbol ParameterSymbol { get; set; }
    }
}
