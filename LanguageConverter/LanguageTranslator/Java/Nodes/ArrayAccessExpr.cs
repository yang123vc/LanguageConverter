using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class ArrayAccessExpr : IStmt
    {
        public IStmt Expression { get; set; }
        public IStmt[] IndexExpressions { get; set; }
        public StmtKind Kind { get {return StmtKind.ArrayAccessExpr;} }
    }
}
