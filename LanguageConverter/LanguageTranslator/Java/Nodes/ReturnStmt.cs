using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class ReturnStmt : IStmt
    {
        public IStmt Expr { get; set; }
        public StmtKind Kind { get {return StmtKind.ReturnStmt;} }
    }
}
