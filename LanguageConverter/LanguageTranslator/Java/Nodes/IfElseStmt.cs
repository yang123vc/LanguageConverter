using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class IfElseStmt : IStmt
    {
        public IStmt Condition { get; set; }
        public IStmt ThenBody { get; set; }
        public IStmt ElseBody { get; set; }
        public StmtKind Kind { get {return StmtKind.IfElseStmt;} }
    }
}
