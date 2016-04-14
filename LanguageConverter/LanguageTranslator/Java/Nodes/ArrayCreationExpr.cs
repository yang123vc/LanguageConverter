using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class ArrayCreationExpr : IStmt
    {
        public IStmt Rank { get; set; }
        public StmtKind Kind { get {return StmtKind.ArrayCreationExpr;} }
    }
}
