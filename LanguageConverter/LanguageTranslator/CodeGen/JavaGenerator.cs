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
//                case DeclarationKind.Method:
//                    return TraverseMethod(declaration as JavaMethod);
//                case DeclarationKind.Ctor:
//                    return TraverseCtor(declaration as CtorMethod);
//                case DeclarationKind.Field:
//                    return TraverseField(declaration as JavaField);
            }
            return default(string);
        }

        public string TraverseClass(JavaClass javaClass)
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
                ? string.Format("class {0} {{ {1} }}", javaClass.TypeSymbol, body)
                : string.Format("class {0} extends {1} {{ {2} }}", javaClass.TypeSymbol, baseClass, body);
        }
    }
}
