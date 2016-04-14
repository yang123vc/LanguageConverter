using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class BaseCtorCallExpr : IStmt
    {
        public IStmt[] Arguments { get; set; }
        public StmtKind Kind { get {return StmtKind.BaseCtorCallExpr;} }
    }
}
