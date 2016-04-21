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
        private readonly AccessibilityResolver accessibilityResolver = new AccessibilityResolver();
        private readonly StatementTraverser statementTraverser = new StatementTraverser();

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
                case DeclarationKind.Method:
                    return TraverseMethod(declaration as JavaMethod);
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
                baseClass = baseType.Name == "Object" ? "" : baseType.Name;
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
            var declaredAccesibility = accessibilityResolver.ResolveAccesebility(javaClass.DeclaredAccessibility);
            return string.IsNullOrEmpty(baseClass)
                ? string.Format("{0} class {1} {{ {2} }}", declaredAccesibility, javaClass.TypeSymbol, body).Trim()
                : string.Format("{0} class {1} extends {2} {{ {3} }}", declaredAccesibility, javaClass.TypeSymbol, baseClass, body).Trim();
        }

        private string TraverseField(JavaField javaField)
        {
            var declaredAccesibility = accessibilityResolver.ResolveAccesebility(javaField.DeclaredAccessibility);
            var staticStr = javaField.IsStatic ? "static" : "";
            var fieldStr = string.Format("{0} {1} {2} {3}", declaredAccesibility, staticStr, javaField.TypeSymbol, javaField.FieldName);
            return javaField.Initialization != null
                ? string.Format("{0} = {1}", fieldStr, statementTraverser.TraverseStmt(javaField.Initialization)).Trim()
                : fieldStr.Trim();
        }

        private string TraverseMethod(JavaMethod javaMethod)
        {
            var symbol = javaMethod.MethodSymbol;
            var declaredAccesibility = accessibilityResolver.ResolveAccesebility(javaMethod.DeclaredAccessibility);
            var staticStr = javaMethod.IsStatic ? "static" : "";
            return string.Format("{0} {1} {2} {3}({4}) {5}",
                declaredAccesibility,
                staticStr,
                symbol.ReturnType,
                javaMethod.Name,
                string.Join(", ", javaMethod.Parameters.Select(GetArgument)),
                statementTraverser.TraverseStmt(javaMethod.Body));
        }

        private object GetArgument(MethodParameterInfo methodParameterInfo)
        {
            return string.Format("{0} {1}", methodParameterInfo.ParameterSymbol.Type, methodParameterInfo.Name);
        }
    }
}
