using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class BreakStmt : IStmt
    {
        public StmtKind Kind { get {return StmtKind.BreakStmt;} }
    }
}
