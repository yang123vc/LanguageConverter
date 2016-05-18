using System.Collections.Generic;
using System.Linq;
using LanguageTranslator.Java;
using LanguageTranslator.Java.Interfaces;
using LanguageTranslator.Java.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageTranslator
{
    public static class TranslatorHelper
    {
        public static JavaMethod TranslateMethod(SemanticModel semanticModel, MethodDeclarationSyntax node, CSharpSyntaxVisitor<IStmt> statementVisitor)
        {
            var methodSymbol = semanticModel.GetDeclaredSymbol(node);
            var isStatic = methodSymbol.IsStatic;
            var body = methodSymbol.IsAbstract
                ? new CompoundStmt { Statements = new IStmt[0] }
                : statementVisitor.Visit(node.Body);
            return new JavaMethod
            {
                Name = methodSymbol.Name,
                Parameters = methodSymbol.Parameters.Select(parameterSymbol => new MethodParameterInfo
                {
                    Name = parameterSymbol.Name,
                    DefaultValue = GetParameterDefaultValue(parameterSymbol),
                    ParameterSymbol = parameterSymbol
                }).ToArray(),
                Body = body,
                IsStatic = isStatic,
                IsAbstract = methodSymbol.IsAbstract,
                MethodSymbol = methodSymbol,
                DeclaredAccessibility = methodSymbol.DeclaredAccessibility
            };
        }

        public static string GetParameterDefaultValue(IParameterSymbol parameter)
        {
            if (!parameter.HasExplicitDefaultValue)
                return null;
            var defaultValue = parameter.ExplicitDefaultValue;
            return defaultValue == null
                ? "null"
                : defaultValue.ToString();
        }

        public static IEnumerable<VariableDeclaratorSyntax> GetFields(SyntaxNode node)
        {
            var fieldDeclarations = node.DescendantNodes().OfType<FieldDeclarationSyntax>();
            return fieldDeclarations.SelectMany(fieldDecl => fieldDecl.DescendantNodes().OfType<VariableDeclaratorSyntax>());
        }

        public static IEnumerable<PropertyDeclarationSyntax> GetProperties(SyntaxNode node)
        {
            var propertyDeclarations = node.DescendantNodes().OfType<PropertyDeclarationSyntax>();
            return propertyDeclarations;
        }

        public static JavaField TranslateField(SemanticModel semanticModel, VariableDeclaratorSyntax node, CSharpSyntaxVisitor<IStmt> statementVisitor)
        {
            var symbol = semanticModel.GetDeclaredSymbol(node);
            return new JavaField
            {
                FieldName = node.Identifier.ToString(),
                IsStatic = symbol.IsStatic,
                Initialization = node.Initializer != null ? statementVisitor.Visit(node.Initializer) : null,
                TypeSymbol = SymbolHelper.GetVariableSymbol(symbol),
                DeclaredAccessibility = symbol.DeclaredAccessibility
            };
        }

        public static JavaField TranslateProp(SemanticModel semanticModel, PropertyDeclarationSyntax node, CSharpSyntaxVisitor<IStmt> statementVisitor)
        {
            var symbol = semanticModel.GetDeclaredSymbol(node);
            return new JavaField
            {
                FieldName = node.Identifier.ToString(),
                IsStatic = symbol.IsStatic,
                Initialization = node.Initializer != null ? statementVisitor.Visit(node.Initializer) : null,
                TypeSymbol = SymbolHelper.GetVariableSymbol(symbol),
                DeclaredAccessibility = symbol.DeclaredAccessibility
            };
        }
    }
}
