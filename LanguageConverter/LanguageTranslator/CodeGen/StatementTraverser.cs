using System.Linq;
using System.Text;
using LanguageTranslator.Java;
using LanguageTranslator.Java.Interfaces;
using LanguageTranslator.Java.Nodes;

namespace LanguageTranslator.CodeGen
{
    public class StatementTraverser
    {
        private readonly JavaTypeResolver javaTypeResolver;

        public StatementTraverser(JavaTypeResolver javaTypeResolver)
        {
            this.javaTypeResolver = javaTypeResolver;
        }

        public string TraverseStmt(IStmt stmt)
        {
            if (stmt == null) return "";
            switch (stmt.Kind)
            {
                case StmtKind.LiteralExpr:
                    var literalExpr = stmt as LiteralExpr;
                    return literalExpr.Value;
                case StmtKind.ObjectCreationExpr:
                    return TraverseObjectCreationExpr(stmt as ObjectCreationExpr);
                case StmtKind.CompoundStmt:
                    var compoundStmt = stmt as CompoundStmt;
                    return compoundStmt.Statements.Length == 0 ? ";" 
                           : string.Format("{{ {0} }}", string.Join(" ", compoundStmt.Statements.Select(GenerateSeparateStmt)));
                case StmtKind.ReturnStmt:
                    var returnStmt = stmt as ReturnStmt;
                    return returnStmt.Expr != null
                        ? string.Format("return {0}", TraverseStmt(returnStmt.Expr))
                        : "return";
                case StmtKind.UnaryExpr:
                    var unaryExpr = stmt as UnaryExpr;
                    return unaryExpr.IsPrefixExpr
                        ? string.Format("{0}{1}", unaryExpr.Operation, TraverseStmt(unaryExpr.Expression))
                        : string.Format("{0}{1}", TraverseStmt(unaryExpr.Expression), unaryExpr.Operation);
                case StmtKind.CompoundLocalDeclStmt:
                    var compoundLocalDeclStmt = stmt as CompoundLocalDeclStmt;
                    return string.Join(" ", compoundLocalDeclStmt.Declarations.Select(TraverseStmt));
                case StmtKind.LocalDeclStmt:
                    var localDeclStmt = stmt as LocalDeclStmt;
                    return localDeclStmt.Initialization != null
                        ? string.Format("{0} {1} = {2}", javaTypeResolver.Resolve(localDeclStmt.TypeSymbol), localDeclStmt.DeclName, TraverseStmt(localDeclStmt.Initialization))
                        : string.Format("{0} {1}", javaTypeResolver.Resolve(localDeclStmt.TypeSymbol), localDeclStmt.DeclName);
                case StmtKind.BaseCtorCallExpr:
                    var baseCtorCallExpr = stmt as BaseCtorCallExpr;
                    return string.Format("super({0})", string.Join(", ", baseCtorCallExpr.Arguments.Select(TraverseStmt)));
                case StmtKind.IdentifierExpr:
                    var identifierExpr = stmt as IdentifierExpr;
                    return identifierExpr.Identifier;
                case StmtKind.BinaryExpr:
                    var binaryExpr = stmt as BinaryExpr;
                    return string.Format("{0} {1} {2}", TraverseStmt(binaryExpr.Left), binaryExpr.Operation, TraverseStmt(binaryExpr.Right));
                case StmtKind.BreakStmt:
                    return "break";
                case StmtKind.ParenExpr:
                    var parenExpr = stmt as ParenExpr;
                    return string.Format("({0})", TraverseStmt(parenExpr.Expression));
                case StmtKind.ThisExpr:
                    return "this";
                case StmtKind.BaseExpr:
                    return "super";
                case StmtKind.Unknown:
                    return "";
                case StmtKind.ArrayAccessExpr:
                    var arrayAccessExpr = stmt as ArrayAccessExpr;
                    return string.Format("{0}[{1}]", TraverseStmt(arrayAccessExpr.Expression),
                        string.Join(", ", arrayAccessExpr.IndexExpressions.Select(TraverseStmt)));
                //                case StmtKind.ArrayCreationExpr:
                //                    break;
                case StmtKind.IndexExpr:
                    var indexExpr = stmt as IndexExpr;
                    return string.Format("{{{0}}}", string.Join(", ", indexExpr.Elements.Select(TraverseStmt)));
                case StmtKind.CallExpr:
                    var callExpr = stmt as CallExpr;
                    return string.Format("{0}({1})", TraverseStmt(callExpr.Expression), string.Join(", ", callExpr.Arguments.Select(TraverseStmt)));
                case StmtKind.ConditionalExpr:
                    var conditionalExpr = stmt as ConditionalExpr;
                    return string.Format("{0} ? {1} : {2}",
                        TraverseStmt(conditionalExpr.Condition),
                        TraverseStmt(conditionalExpr.ThenStmt),
                        TraverseStmt(conditionalExpr.ElseStmt));
                case StmtKind.ForEachStmt:
                    var forEachStmt = stmt as ForEachStmt;
                    return string.Format("for({0} : {1}) {2}", 
                                         TraverseStmt(forEachStmt.Iterator), 
                                         TraverseStmt(forEachStmt.SourceContainer), 
                                         GenerateSeparateStmt(forEachStmt.Body));
                case StmtKind.ForStmt:
                    var forStmt = stmt as ForStmt;
                    var initialization = forStmt.Declaration != null
                        ? TraverseStmt(forStmt.Declaration)
                        : string.Join(", ", forStmt.Initializers.Select(TraverseStmt));
                    return string.Format("for({0}; {1}; {2}) {3}",
                        initialization, TraverseStmt(forStmt.Condition),
                        string.Join(", ", forStmt.Incrementors.Select(TraverseStmt)),
                        GenerateSeparateStmt(forStmt.Body));
                case StmtKind.IfElseStmt:
                    return TraverseIfElseStmt(stmt as IfElseStmt);
                case StmtKind.MemberAccessExpr:
                    var memberAccessExpr = stmt as MemberAccessExpr;
                    return string.Format("{0}.{1}", TraverseStmt(memberAccessExpr.ObjectExpr), TraverseStmt(memberAccessExpr.MemberExpr));
//                case StmtKind.SwitchCaseStmt:
//                    break;
//                case StmtKind.SwitchStmt:
//                    break;
                case StmtKind.WhileStmt:
                    var whileStmt = stmt as WhileStmt;
                    return string.Format("while({0}) {1}", TraverseStmt(whileStmt.Condition), GenerateSeparateStmt(whileStmt.Body));
                case StmtKind.DoWhileStmt:
                    var doWhileStmt = stmt as DoWhileStmt;
                    return string.Format("do {0} while({1});", GenerateSeparateStmt(doWhileStmt.Body), TraverseStmt(doWhileStmt.Condition));
                //                case StmtKind.FunctionLiteral:
                //                    break;
                //                case StmtKind.ArrayLiteralExpr:
                //                    break;
                case StmtKind.ThrowStmt:
                    var throwStmt = stmt as ThrowStmt;
                    return string.Format("throw {0}", TraverseStmt(throwStmt.Expression));
                default:
                    return "";
            }
        }

        private string TraverseIfElseStmt(IfElseStmt ifElseStmt)
        {
            return ifElseStmt.ElseBody != null
                ? string.Format("if({0}) {1} else {2}", TraverseStmt(ifElseStmt.Condition),
                    GenerateSeparateStmt(ifElseStmt.ThenBody),
                    GenerateSeparateStmt(ifElseStmt.ElseBody))
                : string.Format("if({0}) {1}", TraverseStmt(ifElseStmt.Condition),
                    GenerateSeparateStmt(ifElseStmt.ThenBody));
        }

        private string TraverseObjectCreationExpr(ObjectCreationExpr objectCreationExpr)
        {
            var arguments = string.Join(", ", objectCreationExpr.Arguments.Select(TraverseStmt));
            return string.Format("new {0}({1})", javaTypeResolver.Resolve(objectCreationExpr.TypeInformation, true), arguments);
        }

        private object GenerateSeparateStmt(IStmt stmt)
        {
            var body = new StringBuilder();
            body.Append(TraverseStmt(stmt));
            var compositeStatements = new[]
            {
                StmtKind.CompoundStmt,
                StmtKind.IfElseStmt,
                StmtKind.ForEachStmt,
                StmtKind.ForStmt,
                StmtKind.WhileStmt,
                StmtKind.SwitchStmt
            };
            if (!compositeStatements.Contains(stmt.Kind))
                body.Append(';');
            return body.ToString();
        }
    }
}
