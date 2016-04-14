using System.Linq;
using Microsoft.CodeAnalysis;

namespace LanguageTranslator
{
    public static class SymbolHelper
    {
        public static ITypeSymbol GetVariableSymbol(ISymbol symbol)
        {
            var fieldSymbol = symbol as IFieldSymbol;
            if (fieldSymbol != null)
                return fieldSymbol.Type;
            var localSymbol = symbol as ILocalSymbol;
            if (localSymbol != null)
                return localSymbol.Type;
            return null;
        }

    }
}