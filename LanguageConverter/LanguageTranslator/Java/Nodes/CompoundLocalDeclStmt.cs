using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class CompoundLocalDeclStmt : IStmt
    {
        public IStmt[] Declarations { get; set; }
        public StmtKind Kind { get {return StmtKind.CompoundLocalDeclStmt;} }
    }
}
