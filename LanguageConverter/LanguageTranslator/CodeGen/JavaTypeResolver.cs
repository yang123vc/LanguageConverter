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

        public string Resolve(TypeInformation typeInformation, bool isGenericArg = false)
        {
            if (typeInformation == null)
                return "";
            if (typeInformation.TypeSymbol != null)
                return Resolve(typeInformation.TypeSymbol, isGenericArg);
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

        public string Resolve(ITypeSymbol symbol, bool isGenericArg = false)
        {
            if (symbol == null)
                return "";            
            var typeName = symbol.Name;
            string outputType;
            if (TryResolveBuiltinType(typeName, out outputType, isGenericArg))
                return outputType;
            if (TryResolveArrayType(symbol, out outputType))
                return outputType;
            if (!TryResolveClientType(symbol, out outputType))
                outputType = typeName;            
            var genericTypeArguments = GetGenericTypeArguments(symbol);
            return genericTypeArguments.Any()
                ? string.Format("{0}<{1}>", outputType, string.Join(",", genericTypeArguments))
                : outputType;
        }

        private static bool TryResolveBuiltinType(string typeName, out string resolvedType, bool isGenericArg = false)
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
            var builtinGenericTypes = new Dictionary<string, string>
            {
                {"boolean", "Boolean"},
                {"char", "Character"},
                {"byte", "Byte"},
                {"double", "Double"},
                {"single", "Float"},
                {"int16", "Short"},
                {"uint16", "Short"},
                {"int32", "Integer"},
                {"uint32", "Integer"},
                {"int64", "Long"},
                {"uint64", "Long"},
                {"object", "Object"}
            };
            return isGenericArg 
                ? builtinGenericTypes.TryGetValue(typeName.ToLower(), out resolvedType) 
                : builtinTypes.TryGetValue(typeName.ToLower(), out resolvedType);
        }

        private bool TryResolveClientType(ITypeSymbol symbol, out string resolvedType)
        {
            resolvedType = resolvers
                .Select(resolver => resolver.Resolve(symbol))
                .FirstOrDefault(output => !string.IsNullOrEmpty(output));
            return !string.IsNullOrEmpty(resolvedType);
        }

        private bool TryResolveArrayType(ITypeSymbol symbol, out string resolvedType)
        {
            var arrayTypeSymbol = symbol as IArrayTypeSymbol;
            if (arrayTypeSymbol != null)
            {
                var rank = arrayTypeSymbol.Rank;
                resolvedType = $"{Resolve(arrayTypeSymbol.ElementType)}{string.Join("", Enumerable.Repeat("[]", rank))}";
                return true;
            }
            resolvedType = null;
            return false;
        }

        private string[] GetGenericTypeArguments(ITypeSymbol symbol)
        {
            var namedTypeSymbol = symbol as INamedTypeSymbol;
            if (namedTypeSymbol != null)
            {
                return namedTypeSymbol.TypeArguments.Select(type => Resolve(type, true)).ToArray();
            }
            return new string[0];
        }
    }
}
