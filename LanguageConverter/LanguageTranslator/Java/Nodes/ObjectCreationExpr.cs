using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    public class ObjectCreationExpr : IStmt
    {
        public TypeInformation TypeInformation { get; set; }
        public IStmt[] Arguments { get; set; }
        public StmtKind Kind { get {return StmtKind.ObjectCreationExpr;} }
    }
}
