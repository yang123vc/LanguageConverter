using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class SwitchCaseStmt : IStmt
    {
        public IStmt[] Labels { get; set; }
        public IStmt[] Statements { get; set; }
        public StmtKind Kind { get {return StmtKind.SwitchCaseStmt;} }
    }
}
