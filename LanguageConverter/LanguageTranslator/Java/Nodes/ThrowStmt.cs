using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class ThrowStmt : IStmt
    {
        public IStmt Expression { get; set; }
        public StmtKind Kind { get {return StmtKind.ThrowStmt;} }
    }
}