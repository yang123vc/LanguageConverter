using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class IndexExpr : IStmt
    {
        public IStmt[] Elements { get; set; }
        public StmtKind Kind { get {return StmtKind.IndexExpr;} }
    }
}
