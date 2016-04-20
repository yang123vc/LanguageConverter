using System;
using System.Linq;
using System.Text;
using LanguageTranslator.CodeGen.Interfaces;
using LanguageTranslator.Java;
using LanguageTranslator.Java.Interfaces;
using LanguageTranslator.Java.Nodes;

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
            return string.IsNullOrEmpty(baseClass)
                ? string.Format("{0} class {1} {{ {2} }}", javaClass.TypeSymbol.DeclaredAccessibility, javaClass.TypeSymbol, body).Trim()
                : string.Format("{0} class {1} extends {2} {{ {3} }}", javaClass.TypeSymbol.DeclaredAccessibility, javaClass.TypeSymbol, baseClass, body).Trim();
        }

        private string TraverseField(JavaField javaField)
        {
            var staticStr = javaField.IsStatic ? "static" : "";
            var fieldStr = string.Format("{0} {1} {2} {3}", javaField.TypeSymbol.DeclaredAccessibility, staticStr, javaField.TypeSymbol, javaField.FieldName);
            return fieldStr.Trim();
//            return javaField.Initialization != null
//                ? string.Format("{0} = {1}", fieldStr, TraverseStmt(javaField.Initialization)).Trim()
//                : fieldStr.Trim();
        }
    }
}
