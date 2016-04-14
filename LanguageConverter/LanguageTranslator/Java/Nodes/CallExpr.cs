using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class CallExpr : IStmt
    {
        public IStmt Expression { get; set; }
        public IStmt[] Arguments { get; set; }
        public StmtKind Kind { get {return StmtKind.CallExpr;} }
    }
}
