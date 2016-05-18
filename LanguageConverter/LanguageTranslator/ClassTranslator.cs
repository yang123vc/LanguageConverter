using System;
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

        public JavaClass Translate(ClassDeclarationSyntax declarationNode, CSharpSyntaxVisitor<IStmt> statementTranslator)
        {
            var symbol = semanticModel.GetDeclaredSymbol(declarationNode);
            if (symbol == null)
                throw new Exception("Cannot build semantic information for class type");            
            var descendantNodes = declarationNode.DescendantNodes().ToArray();
            var methods = descendantNodes.OfType<MethodDeclarationSyntax>()
                                         .Select(method => TranslatorHelper.TranslateMethod(semanticModel, method, statementTranslator)).ToArray();
            var ctors = descendantNodes.OfType<ConstructorDeclarationSyntax>().Select(ctor => TranslateCtor(ctor, statementTranslator)).ToArray();
            var fields = TranslatorHelper.GetFields(declarationNode)
                                         .Select(node => TranslatorHelper.TranslateField(semanticModel, node, statementTranslator));
            var props = TranslatorHelper.GetProperties(declarationNode)
                                         .Select(node => TranslatorHelper.TranslateProp(semanticModel, node, statementTranslator));
            var className = declarationNode.Identifier.ToString();
            return new JavaClass
            {
                Name = className,
                Methods = ctors.Concat<IMethod>(methods).ToArray(),
                Fields = props.Concat(fields).ToArray(),
                TypeSymbol = symbol,
                DeclaredAccessibility = symbol.DeclaredAccessibility,
                IsAbstract = symbol.IsAbstract
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
                    DefaultValue = TranslatorHelper.GetParameterDefaultValue(parameterSymbol),
                    ParameterSymbol = parameterSymbol
                }).ToArray(),
                Body = statementVisitor.Visit(node.Body),
                MethodSymbol = methodSymbol,
                BaseCtorCallExpr = baseCtorCall,
                DeclaredAccessibility = methodSymbol.DeclaredAccessibility
            };
        }
    }
}
