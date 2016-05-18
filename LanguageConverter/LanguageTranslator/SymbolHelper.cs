using System.Linq;
using Microsoft.CodeAnalysis;

namespace LanguageTranslator
{
    public static class SymbolHelper
    {
        public static ITypeSymbol GetVariableSymbol(ISymbol symbol)
        {
            var propSymbol = symbol as IPropertySymbol;
            if (propSymbol != null)
                return propSymbol.Type;
            var fieldSymbol = symbol as IFieldSymbol;
            if (fieldSymbol != null)
                return fieldSymbol.Type;
            var localSymbol = symbol as ILocalSymbol;
            if (localSymbol != null)
                return localSymbol.Type;
            return null;
        }

        public static bool IsAssignableFrom(ISymbol symbol, string typeName)
        {
            if (symbol == null)
                return false;
            return symbol.Name == typeName || IsAssignableFrom(symbol.ContainingType, typeName);
        }

        public static bool IsAssignableFrom(ITypeSymbol symbol, string typeName)
        {
            if (symbol == null)
                return false;
            if (symbol.Name == typeName)
                return true;
            return IsAssignableFrom(symbol.BaseType, typeName) || Enumerable.Any(symbol.AllInterfaces, interfaceType => interfaceType.Name == typeName);
        }
    }
}