using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class CompoundStmt : IStmt
    {
        public IStmt[] Statements { get; set; }
        public StmtKind Kind { get {return StmtKind.CompoundStmt;} }
    }
}
