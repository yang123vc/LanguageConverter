using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class LiteralExpr : IStmt
    {
        public StmtKind Kind { get {return StmtKind.LiteralExpr;} }
        public string Value { get; set; }
    }
}
