using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class IdentifierExpr : IStmt
    {
        public string Identifier { get; set; }
        public StmtKind Kind { get {return StmtKind.IdentifierExpr;} }
    }
}
