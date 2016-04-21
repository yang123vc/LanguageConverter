using Microsoft.CodeAnalysis;

namespace LanguageTranslator.CodeGen
{
    public class AccessibilityResolver
    {
        public string ResolveAccesebility(Accessibility declaredAccessibility)
        {
            switch (declaredAccessibility)
            {
                case Accessibility.NotApplicable:
                    return "";
                case Accessibility.Private:
                    return "private";
                case Accessibility.ProtectedAndInternal:
                    return "protected";
                case Accessibility.Protected:
                    return "protected";
                case Accessibility.Internal:
                    return "";
                case Accessibility.ProtectedOrInternal:
                    return "protected";
                case Accessibility.Public:
                    return "public";
                default:
                    return "";
            }
        }
    }
}
