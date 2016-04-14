using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class ForStmt : IStmt
    {
        public IStmt[] Initializers { get; set; }        
        public IStmt Condition { get; set; }
        public IStmt[] Incrementors { get; set; }
        public IStmt Body { get; set; }
        public StmtKind Kind { get { return StmtKind.ForStmt; } }
    }
}
