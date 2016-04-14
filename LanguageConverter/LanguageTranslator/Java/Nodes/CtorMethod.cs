using LanguageTranslator.Java.Interfaces;
using Microsoft.CodeAnalysis;

namespace LanguageTranslator.Java.Nodes
{
    public class CtorMethod : IMethod
    {        
        public string Name { get { return "constructor"; } }
        public MethodParameterInfo[] Parameters { get; set; }
        public IStmt Body { get; set; }
        public bool IsStatic { get; set; }
        public IMethodSymbol MethodSymbol { get; set; }
        public DeclarationKind Kind { get { return DeclarationKind.Ctor; } }
        public BaseCtorCallExpr BaseCtorCallExpr { get; set; }
    }
}
