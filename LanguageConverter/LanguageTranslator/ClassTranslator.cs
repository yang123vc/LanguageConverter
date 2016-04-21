using System;
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
    class ClassTranslator
    {
        private readonly SemanticModel semanticModel;

        public ClassTranslator(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        public JavaClass Translate(ClassDeclarationSyntax declarationNode, StatementTranslator statementTranslator)
        {
            var symbol = semanticModel.GetDeclaredSymbol(declarationNode);
            if (symbol == null)
                throw new Exception("Cannot build semantic information for class type");            
            var descendantNodes = declarationNode.DescendantNodes().ToArray();
            var methods = descendantNodes.OfType<MethodDeclarationSyntax>().Select(method => TranslateMethod(method, statementTranslator)).ToArray();
            var ctors = descendantNodes.OfType<ConstructorDeclarationSyntax>().Select(ctor => TranslateCtor(ctor, statementTranslator)).ToArray();
            var fields = GetFields(declarationNode).Select(node => TranslateField(node, statementTranslator));
            var className = declarationNode.Identifier.ToString();
            return new JavaClass
            {
                Name = className,
                Methods = ctors.Concat<IMethod>(methods).ToArray(),
                Fields = fields.ToArray(),
                TypeSymbol = symbol,
                DeclaredAccessibility = symbol.DeclaredAccessibility
            };
        }

        private CtorMethod TranslateCtor(ConstructorDeclarationSyntax node, CSharpSyntaxVisitor<IStmt> statementVisitor)
        {
            var methodSymbol = semanticModel.GetDeclaredSymbol(node);
            BaseCtorCallExpr baseCtorCall = null;
            if (node.Initializer != null)
            {
                baseCtorCall = new BaseCtorCallExpr
                {
                    Arguments = node.Initializer.ArgumentList.Arguments.Select(statementVisitor.Visit).ToArray()
                };
            }
            return new CtorMethod
            {
                Name = methodSymbol.ReceiverType.Name,
                Parameters = methodSymbol.Parameters.Select(parameterSymbol => new MethodParameterInfo
                {
                    Name = parameterSymbol.Name,
                    DefaultValue = GetParameterDefaultValue(parameterSymbol),
                    ParameterSymbol = parameterSymbol
                }).ToArray(),
                Body = statementVisitor.Visit(node.Body),
                MethodSymbol = methodSymbol,
                BaseCtorCallExpr = baseCtorCall,
                DeclaredAccessibility = methodSymbol.DeclaredAccessibility
            };
        }

        private JavaMethod TranslateMethod(MethodDeclarationSyntax node, CSharpSyntaxVisitor<IStmt> statementVisitor)
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
                MethodSymbol = methodSymbol,
                DeclaredAccessibility = methodSymbol.DeclaredAccessibility
            };
        }

        private static string GetParameterDefaultValue(IParameterSymbol parameter)
        {
            if (!parameter.HasExplicitDefaultValue)
                return null;
            var defaultValue = parameter.ExplicitDefaultValue;
            return defaultValue == null
                ? "null"
                : defaultValue.ToString();
        }

        private static IEnumerable<VariableDeclaratorSyntax> GetFields(ClassDeclarationSyntax node)
        {
            var fieldDeclarations = node.DescendantNodes().OfType<FieldDeclarationSyntax>();
            return fieldDeclarations.SelectMany(fieldDecl => fieldDecl.DescendantNodes().OfType<VariableDeclaratorSyntax>());
        }

        private JavaField TranslateField(VariableDeclaratorSyntax node, CSharpSyntaxVisitor<IStmt> statementVisitor)
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
