using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageTranslator.CodeGen.Interfaces;
using LanguageTranslator.ExtensionPoints;
using LanguageTranslator.Java;
using Microsoft.CodeAnalysis.CSharp;

namespace LanguageTranslator
{
    public class TranslationRunner
    {
        private readonly ICustomCodeGenerator codeGenerator;
        private readonly IExtensionPoint[] extensionPoints;
        private readonly TranslationOptions options;
        private readonly string commonJavaImports = "import java.util.*;\r\n" +
                                                    "import javafx.util.*;\r\n" +
                                                    "import java.util.stream.*;\r\n" +
                                                    "import java.util.function.*;\r\n";

        public TranslationRunner(ICustomCodeGenerator codeGenerator, IExtensionPoint[] extensionPoints, TranslationOptions options)
        {
            this.codeGenerator = codeGenerator;
            this.extensionPoints = extensionPoints;
            this.options = options;            
        }

        public void Translate(string inputDir, string outputDir, string[] dependencyAssemblies)
        {
            var inputFiles = Directory.GetFiles(inputDir, "*.cs", SearchOption.AllDirectories);
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
            if (options.MultiThreading)
            {
                Parallel.ForEach(inputFiles, inputFile => TranslateFile(inputFile, outputDir, dependencyAssemblies));
            }
            else
            {
                foreach (var inputFile in inputFiles)
                {
                    TranslateFile(inputFile, outputDir, dependencyAssemblies);
                }
            }
        }

        private void TranslateFile(string inputFile, string outputDir, string[] dependencyAssemblies)
        {
            var inputFileName = Path.GetFileName(inputFile);
            var outputFile = Path.Combine(outputDir, Path.ChangeExtension(inputFileName, codeGenerator.FileExtension));
            var javaSyntaxTree = GetSyntaxTree(inputFile, dependencyAssemblies);
            var generatedCode = codeGenerator.Generate(javaSyntaxTree);
            Console.WriteLine($"\t{inputFileName} \ttranslated");
            if (options.IsBeautify)
            {
                Beautify(generatedCode, outputFile);
                return;
            }
            if (!string.IsNullOrEmpty(generatedCode))
            {
                File.WriteAllText(outputFile, string.Concat(commonJavaImports, generatedCode), Encoding.UTF8);
            }                        
        }

        private void Beautify(string code, string outputFile)
        {
            var beatifierPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "beautifier");
            var beatifierExePath = Path.Combine(beatifierPath, "uncrustify.exe");
            var confingFileName = "sun.cfg";
            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo
            {                
                WorkingDirectory = beatifierPath,
                FileName = beatifierExePath,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = string.Format("-c {0} -o {1} -l java", confingFileName, outputFile)
            };
            proc.Start();
            proc.StandardInput.WriteLine(string.Concat(commonJavaImports, code));
            proc.Close();
        }

        private JavaSyntaxTree GetSyntaxTree(string inputFile, string[] dependencyAssemblies)
        {
            var csSyntaxTree = SyntaxFactory.ParseSyntaxTree(File.ReadAllText(inputFile));
            var semanticModel = SemanticModelBuilder.Build(csSyntaxTree, dependencyAssemblies);
            var moduleTranslator = new ModuleTranslator(csSyntaxTree, semanticModel, extensionPoints.ToArray());
            return moduleTranslator.Translate();
        }
    }
}
