using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LanguageTranslator.CodeGen;
using LanguageTranslator.ExtensionPoints;
using LanguageTranslator.ExtensionPoints.Net;

namespace LanguageTranslator
{
    class EntryPoint
    {
        static void Main(string[] arguments)
        {
            if (arguments.Length < 3)
            {
                Console.WriteLine("Usage: <input-directory> <output-directory> <dependency-assemblies-directory>");
                return;
            }
            var inputDirectory = arguments[0];
            var outputDirectory = arguments[1];
            var assembliesDirectory = arguments[2];
            var dependencyAssemblies = arguments.Skip(3).Select(assemblyName => Path.Combine(assembliesDirectory, assemblyName)).ToArray();
            Console.WriteLine("AutoScript translation begin....");
            var watcher = new Stopwatch();
            watcher.Start();
            var extensionPoints = GetExtensionPoints();
            var typeResolvers = extensionPoints.OfType<ITypeResolver>().ToArray();            
            var translator = new TranslationRunner(new JavaGenerator(typeResolvers), extensionPoints,
                new TranslationOptions
                {
                    IsBeautify = true,
                    MultiThreading = true
                });
            translator.Translate(inputDirectory, outputDirectory, dependencyAssemblies);
            Console.WriteLine("AutoScript translation end.");
            watcher.Stop();
            Console.WriteLine("Ellapsed time - {0}ms", watcher.ElapsedMilliseconds);
        }

        private static IExtensionPoint[] GetExtensionPoints()
        {
            return new IExtensionPoint[]
            {
                new StringExtensionPoint(),
                new ConsoleExtensionPoint(),
                new ObjectExtensionPoint(), 
                new ConvertExtensionPoint(), 
                new ArrayExtensionPoint(), 
                new ListExtensionPoint(), 
                new StackExtensionPoint(), 
                new FuncExtensionPoint(), 
            };
        }
    }
}
