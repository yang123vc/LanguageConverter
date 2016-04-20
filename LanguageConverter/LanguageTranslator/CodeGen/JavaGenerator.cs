using System;
using System.Linq;
using System.Text;
using LanguageTranslator.CodeGen.Interfaces;
using LanguageTranslator.Java;
using LanguageTranslator.Java.Interfaces;
using LanguageTranslator.Java.Nodes;
using Microsoft.CodeAnalysis;

namespace LanguageTranslator.CodeGen
{
    public class JavaGenerator : ICustomCodeGenerator
    {
        public string FileExtension { get { return ".java"; } }

        public string Generate(ICustomSyntaxTree syntaxTree)
        {
            var javaCode = new StringBuilder();
            javaCode.AppendLine(string.Join(" ", syntaxTree.Declarations.Select(TraverseDeclaration)));
            return javaCode.ToString();
        }

        private string TraverseDeclaration(IDeclarationNode declaration)
        {
            switch (declaration.Kind)
            {                
                case DeclarationKind.Class:
                    return TraverseClass(declaration as JavaClass);
                case DeclarationKind.Field:
                    return TraverseField(declaration as JavaField);
//                case DeclarationKind.Method:
//                    return TraverseMethod(declaration as JavaMethod);
//                case DeclarationKind.Ctor:
//                    return TraverseCtor(declaration as CtorMethod);                
            }
            return default(string);
        }        

        private string TraverseClass(JavaClass javaClass)
        {
            var baseClass = "";
            var baseType = javaClass.TypeSymbol.BaseType;
            if (baseType != null)
            {
                baseClass = baseType.Name;
            }
            var body = new StringBuilder();
            foreach (var field in javaClass.Fields)
            {
                body.Append(TraverseDeclaration(field));
                body.Append("; ");
            }
            foreach (var method in javaClass.Methods)
            {
                body.Append(TraverseDeclaration(method));
            }
            var declaredAccesibility = ResolveAccesebility(javaClass.DeclaredAccessibility);
            return string.IsNullOrEmpty(baseClass)
                ? string.Format("{0} class {1} {{ {2} }}", declaredAccesibility, javaClass.TypeSymbol, body).Trim()
                : string.Format("{0} class {1} extends {2} {{ {3} }}", declaredAccesibility, javaClass.TypeSymbol, baseClass, body).Trim();
        }

        private string TraverseField(JavaField javaField)
        {
            var declaredAccesibility = ResolveAccesebility(javaField.DeclaredAccessibility);
            var staticStr = javaField.IsStatic ? "static" : "";
            var fieldStr = string.Format("{0} {1} {2} {3}", declaredAccesibility, staticStr, javaField.TypeSymbol, javaField.FieldName);
            return fieldStr.Trim();
//            return javaField.Initialization != null
//                ? string.Format("{0} = {1}", fieldStr, TraverseStmt(javaField.Initialization)).Trim()
//                : fieldStr.Trim();
        }

        private string ResolveAccesebility(Accessibility declaredAccessibility)
        {
            switch (declaredAccessibility)
            {
                case Accessibility.NotApplicable:
                    return "";
                case Accessibility.Private:
                    return "private";
                case Accessibility.ProtectedAndInternal:
                    return "protected";
                case Accessibility.Protected:
                    return "protected";
                case Accessibility.Internal:
                    return "";
                case Accessibility.ProtectedOrInternal:
                    return "protected";
                case Accessibility.Public:
                    return "public";
                default:
                    return "";
            }
        }
    }
}
