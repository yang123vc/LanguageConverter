using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class ParenExpr : IStmt
    {
        public IStmt Expression { get; set; }
        public StmtKind Kind { get {return StmtKind.ParenExpr;} }
    }
}
