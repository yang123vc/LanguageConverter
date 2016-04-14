using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class BinaryExpr : IStmt
    {
        public IStmt Left { get; set; }
        public string Operation { get; set; }
        public IStmt Right { get; set; }
        public StmtKind Kind { get {return StmtKind.BinaryExpr;} }
    }
}
