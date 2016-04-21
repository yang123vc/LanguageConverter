using LanguageTranslator.Java.Interfaces;
using Microsoft.CodeAnalysis;

namespace LanguageTranslator.Java.Nodes
{
    public class JavaMethod : IMethod
    {        
        public string Name { get; set; }
        public MethodParameterInfo[] Parameters { get; set; }
        public IStmt Body { get; set; }
        public bool IsStatic { get; set; }
        public bool IsAbstract { get; set; }
        public IMethodSymbol MethodSymbol { get; set; }
        public DeclarationKind Kind { get {return DeclarationKind.Method;} }
        public Accessibility DeclaredAccessibility { get; set; }
    }
}
