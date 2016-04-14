using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class UnaryExpr : IStmt
    {
        public bool IsPrefixExpr { get; set; }
        public string Operation { get; set; }
        public IStmt Expression { get; set; }
        public StmtKind Kind { get {return StmtKind.UnaryExpr;} }
    }
}
