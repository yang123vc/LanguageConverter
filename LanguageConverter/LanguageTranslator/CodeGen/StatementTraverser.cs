using System.Linq;
using System.Text;
using LanguageTranslator.Java;
using LanguageTranslator.Java.Interfaces;
using LanguageTranslator.Java.Nodes;

namespace LanguageTranslator.CodeGen
{
    internal class StatementTraverser
    {
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
                        ? string.Format("{0} {1} = {2};", localDeclStmt.TypeSymbol, localDeclStmt.DeclName, TraverseStmt(localDeclStmt.Initialization))
                        : string.Format("{0} {1};", localDeclStmt.TypeSymbol, localDeclStmt.DeclName);
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
//                case StmtKind.ArrayAccessExpr:
//                    break;
//                case StmtKind.ArrayCreationExpr:
//                    break;
//                case StmtKind.IndexExpr:
//                    break;
                case StmtKind.CallExpr:
                    var callExpr = stmt as CallExpr;
                    return string.Format("{0}({1})", TraverseStmt(callExpr.Expression), string.Join(", ", callExpr.Arguments.Select(TraverseStmt)));
                case StmtKind.ConditionalExpr:
                    var conditionalExpr = stmt as ConditionalExpr;
                    return string.Format("{0} ? {1} : {2}",
                        TraverseStmt(conditionalExpr.Condition),
                        TraverseStmt(conditionalExpr.ThenStmt),
                        TraverseStmt(conditionalExpr.ElseStmt));
//                case StmtKind.ForEachStmt:
//                    break;
//                case StmtKind.ForStmt:
//                    break;
                case StmtKind.IfElseStmt:
                    return TraverseIfElseStmt(stmt as IfElseStmt);
                case StmtKind.MemberAccessExpr:
                    var memberAccessExpr = stmt as MemberAccessExpr;
                    return string.Format("{0}.{1}", TraverseStmt(memberAccessExpr.ObjectExpr), TraverseStmt(memberAccessExpr.MemberExpr));
//                case StmtKind.SwitchCaseStmt:
//                    break;
//                case StmtKind.SwitchStmt:
//                    break;
//                case StmtKind.WhileStmt:
//                    break;
//                case StmtKind.DoWhileStmt:
//                    break;
//                case StmtKind.FunctionLiteral:
//                    break;
//                case StmtKind.ArrayLiteralExpr:
//                    break;
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
            return string.Format("new {0}({1})", objectCreationExpr.TypeInformation.TypeName, arguments);
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
