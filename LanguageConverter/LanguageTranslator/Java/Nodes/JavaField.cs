using LanguageTranslator.Java.Interfaces;
using Microsoft.CodeAnalysis;

namespace LanguageTranslator.Java.Nodes
{
    public class JavaField : IDeclarationNode
    {
        public bool IsStatic { get; set; }
        public string FieldName { get; set; }
        public IStmt Initialization { get; set; }
        public ITypeSymbol TypeSymbol { get; set; }
        public DeclarationKind Kind { get { return DeclarationKind.Field; } }
    }
}
