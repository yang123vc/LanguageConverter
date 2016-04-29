using System.Collections.Generic;
using System.Linq;
using LanguageTranslator.ExtensionPoints;
using LanguageTranslator.Java;
using Microsoft.CodeAnalysis;

namespace LanguageTranslator.CodeGen
{
    public class JavaTypeResolver
    {
        private readonly ITypeResolver[] resolvers;

        public JavaTypeResolver(ITypeResolver[] resolvers)
        {
            this.resolvers = resolvers;
        }

        public string Resolve(TypeInformation typeInformation)
        {
            if (typeInformation == null)
                return "";
            if (typeInformation.TypeSymbol != null)
                return Resolve(typeInformation.TypeSymbol);
            return typeInformation.TypeName;
        }

        public string Resolve(IMethodSymbol symbol)
        {
            return symbol != null ? Resolve(symbol.ReturnType) : "";
        }

        public string Resolve(IParameterSymbol symbol)
        {
            return symbol != null ? Resolve(symbol.Type) : "";
        }

        public string Resolve(ITypeSymbol symbol)
        {
            if (symbol == null)
                return "";
            var typeName = symbol.Name;
            string outputType;
            if (TryResolveBuiltinType(typeName, out outputType))
                return outputType;
            if (TryResolveClientType(symbol, out outputType))
                return outputType;
            return typeName;
        }

        private static bool TryResolveBuiltinType(string typeName, out string resolvedType)
        {
            var builtinTypes = new Dictionary<string, string>
            {
                {"void", "void"},
                {"boolean", "boolean"},
                {"char", "char"},
                {"byte", "byte"},
                {"double", "double"},
                {"single", "float"},
                {"int16", "short"},
                {"uint16", "short"},
                {"int32", "int"},
                {"uint32", "int"},
                {"int64", "long"},
                {"uint64", "long"},
                {"object", "Object"}
            };
            return builtinTypes.TryGetValue(typeName.ToLower(), out resolvedType);
        }

        private bool TryResolveClientType(ITypeSymbol symbol, out string resolvedType)
        {
            resolvedType = resolvers
                .Select(resolver => resolver.Resolve(symbol))
                .FirstOrDefault(output => !string.IsNullOrEmpty(output));
            return !string.IsNullOrEmpty(resolvedType);
        }
    }
}
