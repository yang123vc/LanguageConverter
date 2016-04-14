using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class DoWhileStmt : IStmt
    {
        public IStmt Condition { get; set; }
        public IStmt Body { get; set; }
        public StmtKind Kind { get {return StmtKind.DoWhileStmt;} }
    }
}
