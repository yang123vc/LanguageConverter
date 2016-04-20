using System;
using System.IO;
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
            var generator = new JavaGenerator();
            IExtensionPoint[] extensionPoints = { new StringExtensionPoint() };
            var options = new TranslationOptions
            {
                IsBeautify = true,
                MultiThreading = false
            };
            var translatorRunner = new TranslationRunner(generator, extensionPoints, options);
            var inDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles");
            var outDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilesOut");
            translatorRunner.Translate(inDir, outDir);
        }
    }
}
