using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class ConditionalExpr : IStmt
    {
        public IStmt Condition { get; set; }
        public IStmt ThenStmt { get; set; }
        public IStmt ElseStmt { get; set; }
        public StmtKind Kind { get {return StmtKind.ConditionalExpr;} }
    }
}
