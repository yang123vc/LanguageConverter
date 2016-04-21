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
            switch (stmt.Kind)
            {
                case StmtKind.LiteralExpr:
                    var expr = stmt as LiteralExpr;
                    return expr != null ? expr.Value : "";
                case StmtKind.ObjectCreationExpr:
                    return TraverseObjectCreationExpr(stmt as ObjectCreationExpr);
                case StmtKind.CompoundStmt:
                    var compoundStmt = stmt as CompoundStmt;
                    return string.Format("{{ {0} }}", string.Join(" ", compoundStmt.Statements.Select(GenerateSeparateStmt)));
                case StmtKind.ReturnStmt:
                    var returnStmt = stmt as ReturnStmt;
                    return string.Format("return {0}", TraverseStmt(returnStmt.Expr));
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
                default:
                    return "";
            }
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
