using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class SwitchStmt : IStmt
    {
        public IStmt Condition { get; set; }
        public SwitchCaseStmt[] Cases { get; set; }
        public StmtKind Kind { get {return StmtKind.SwitchStmt;} }
    }
}
