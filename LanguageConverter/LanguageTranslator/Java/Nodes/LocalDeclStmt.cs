using LanguageTranslator.Java.Interfaces;
using Microsoft.CodeAnalysis;

namespace LanguageTranslator.Java.Nodes
{
    public class LocalDeclStmt : IStmt
    {
        public string DeclName { get; set; }
        public IStmt Initialization { get; set; }
        public ITypeSymbol TypeSymbol { get; set; }
        public StmtKind Kind { get {return StmtKind.LocalDeclStmt;} }
    }
}
