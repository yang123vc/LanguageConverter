using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class ThisExpr : IStmt
    {
        public StmtKind Kind { get { return StmtKind.ThisExpr; } }
    }
}
