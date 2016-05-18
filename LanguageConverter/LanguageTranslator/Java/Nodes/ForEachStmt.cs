using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class ForEachStmt : IStmt
    {
        public IStmt SourceContainer { get; set; }
        public IStmt Iterator { get; set; }
        public IStmt Body { get; set; }
        public StmtKind Kind { get { return StmtKind.ForEachStmt; } }
    }
}
