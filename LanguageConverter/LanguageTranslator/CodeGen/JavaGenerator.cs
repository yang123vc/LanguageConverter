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
                case DeclarationKind.Interface:
                    return TraverseInterface(declaration as JavaInterface);
                case DeclarationKind.Class:
                    return TraverseClass(declaration as JavaClass);
                case DeclarationKind.Field:
                    return TraverseField(declaration as JavaField);
                case DeclarationKind.Ctor:
                    return TraverseCtor(declaration as CtorMethod);
                case DeclarationKind.Method:
                    return TraverseMethod(declaration as JavaMethod);
            }
            return default(string);
        }

        private string TraverseInterface(JavaInterface javaInterface)
        {
            return TraverseClassOrInterface(javaInterface, false);
        }

        private string TraverseClass(JavaClass javaClass)
        {
            return TraverseClassOrInterface(javaClass, true);
        }

        private string TraverseClassOrInterface(IClassOrInterface node, bool isClass)
        {
            var baseClass = "";
            var baseType = node.TypeSymbol.BaseType;
            if (baseType != null)
            {
                baseClass = baseType.Name == "Object" ? "" : baseType.Name;
            }
            var interfaces = node.TypeSymbol.Interfaces;
            var body = new StringBuilder();
            foreach (var field in node.Fields)
            {
                body.Append(TraverseDeclaration(field));
                body.Append("; ");
            }
            foreach (var method in node.Methods)
            {
                body.Append(TraverseDeclaration(method));
            }
            var declaredAccesibility = accessibilityResolver.ResolveAccesebility(node.DeclaredAccessibility);
            string extendsStr;
            string implementStr;
            if (isClass)
            {
                extendsStr = !string.IsNullOrEmpty(baseClass) ? string.Format(" extends {0} ", baseClass) : "";
                implementStr = interfaces.Any() ? string.Format(" implements {0} ", string.Join(", ", interfaces.Select(i => i.Name))) : "";
                return string.Format("{0} class {1}{2}{3} {{ {4} }}", declaredAccesibility, node.TypeSymbol.Name, extendsStr, implementStr, body).Trim();
            }
            extendsStr = interfaces.Any() ? string.Format(" extends {0} ", interfaces.First().Name) : "";
            implementStr = "";
            return string.Format("{0} interface {1}{2}{3} {{ {4} }}", declaredAccesibility, node.TypeSymbol.Name, extendsStr, implementStr, body).Trim();
        }

        private string TraverseField(JavaField javaField)
        {
            var declaredAccesibility = accessibilityResolver.ResolveAccesebility(javaField.DeclaredAccessibility);
            var staticStr = javaField.IsStatic ? "static" : "";
            var fieldStr = string.Format("{0} {1} {2} {3}", declaredAccesibility, staticStr, javaField.TypeSymbol.Name, javaField.FieldName);
            return javaField.Initialization != null
                ? string.Format("{0} = {1}", fieldStr, statementTraverser.TraverseStmt(javaField.Initialization)).Trim()
                : fieldStr.Trim();
        }

        private string TraverseCtor(CtorMethod ctorMethod)
        {
            var javaCtorCode = new StringBuilder();
            var declaredAccesibility = accessibilityResolver.ResolveAccesebility(ctorMethod.DeclaredAccessibility);
            javaCtorCode.AppendFormat("{0} {1}({2}) ", declaredAccesibility, ctorMethod.Name, string.Join(", ", ctorMethod.Parameters.Select(GetArgument)));
            IStmt ctorBody = ctorMethod.Body;
            if (ctorMethod.BaseCtorCallExpr != null)
            {
                ctorBody = CombineStatements(ctorMethod.BaseCtorCallExpr, ctorBody);
            }
            javaCtorCode.Append(statementTraverser.TraverseStmt(ctorBody));
            return javaCtorCode.ToString().Trim();
        }

        private static IStmt CombineStatements(IStmt stmt1, IStmt stmt2)
        {
            var stmt1Block = stmt1 as CompoundStmt;
            var stmt2Block = stmt2 as CompoundStmt;
            IStmt[] statements;
            if (stmt1Block != null && stmt2Block != null)
                statements = stmt1Block.Statements.Concat(stmt2Block.Statements).ToArray();
            else if (stmt1Block != null)
                statements = stmt1Block.Statements.Concat(new[] { stmt2 }).ToArray();
            else if (stmt2Block != null)
                statements = new[] { stmt1 }.Concat(stmt2Block.Statements).ToArray();
            else
                statements = new[] { stmt1, stmt2 };
            return new CompoundStmt { Statements = statements };
        }

        private string TraverseMethod(JavaMethod javaMethod)
        {
            var symbol = javaMethod.MethodSymbol;
            var declaredAccesibility = javaMethod.IsAbstract ? "abstract" : accessibilityResolver.ResolveAccesebility(javaMethod.DeclaredAccessibility);
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
