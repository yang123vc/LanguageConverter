using LanguageTranslator.Java.Interfaces;
using Microsoft.CodeAnalysis;

namespace LanguageTranslator.Java.Nodes
{
    public class CtorMethod : IMethod
    {        
        public string Name { get; set; }
        public MethodParameterInfo[] Parameters { get; set; }
        public IStmt Body { get; set; }
        public bool IsStatic { get; set; }
        public IMethodSymbol MethodSymbol { get; set; }
        public DeclarationKind Kind { get { return DeclarationKind.Ctor; } }
        public Accessibility DeclaredAccessibility { get; set; }
        public BaseCtorCallExpr BaseCtorCallExpr { get; set; }
    }
}
