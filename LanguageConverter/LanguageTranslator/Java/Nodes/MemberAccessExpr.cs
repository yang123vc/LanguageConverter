using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class MemberAccessExpr : IStmt
    {
        public IStmt ObjectExpr { get; set; }
        public IStmt MemberExpr { get; set; }
        public StmtKind Kind { get {return StmtKind.MemberAccessExpr;} }
    }
}
