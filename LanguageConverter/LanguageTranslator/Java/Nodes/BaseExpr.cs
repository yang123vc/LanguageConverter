using LanguageTranslator.Java.Interfaces;

namespace LanguageTranslator.Java.Nodes
{
    class BaseExpr : IStmt
    {
        public StmtKind Kind { get {return StmtKind.BaseExpr;} }
    }
}
