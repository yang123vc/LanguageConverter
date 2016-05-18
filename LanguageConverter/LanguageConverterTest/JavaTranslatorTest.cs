using System;
using System.IO;
using System.Linq;
using LanguageTranslator;
using LanguageTranslator.CodeGen;
using LanguageTranslator.ExtensionPoints;
using LanguageTranslator.ExtensionPoints.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LanguageConverterTest
{
    [TestClass]
    public class JavaTranslatorTest
    {
        [TestMethod]
        public void Test()
        {
            IExtensionPoint[] extensionPoints = { new StringExtensionPoint() };
            var generator = new JavaGenerator(extensionPoints.OfType<ITypeResolver>().ToArray());
            var options = new TranslationOptions
            {
                IsBeautify = true,
                MultiThreading = false
            };
            var translatorRunner = new TranslationRunner(generator, extensionPoints, options);
            var inDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles");
            var outDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesOut");
            translatorRunner.Translate(inDir, outDir, null);
        }
    }
}
