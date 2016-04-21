using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageTranslator.CodeGen;
using LanguageTranslator.CodeGen.Interfaces;
using LanguageTranslator.ExtensionPoints;
using LanguageTranslator.Java;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LanguageTranslator
{
    public class TranslationRunner
    {
        private readonly ICustomCodeGenerator codeGenerator;
        private readonly IExtensionPoint[] extensionPoints;
        private readonly TranslationOptions options;        

        public TranslationRunner(ICustomCodeGenerator codeGenerator, IExtensionPoint[] extensionPoints, TranslationOptions options)
        {
            this.codeGenerator = codeGenerator;
            this.extensionPoints = extensionPoints;
            this.options = options;            
        }

        public void Translate(string inputDir, string outputDir)
        {
            var inputFiles = Directory.GetFiles(inputDir, "*.cs", SearchOption.AllDirectories);
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
            if (options.MultiThreading)
            {
                Parallel.ForEach(inputFiles, inputFile => TranslateFile(inputFile, outputDir));
            }
            else
            {
                foreach (var inputFile in inputFiles)
                {
                    TranslateFile(inputFile, outputDir);
                }
            }
        }

        private void TranslateFile(string inputFile, string outputDir)
        {
            var inputFileName = Path.GetFileName(inputFile);
            var outputFile = Path.Combine(outputDir, Path.ChangeExtension(inputFileName, codeGenerator.FileExtension));
            var javaSyntaxTree = GetSyntaxTree(inputFile);
            var generatedCode = codeGenerator.Generate(javaSyntaxTree);
            if (options.IsBeautify)
            {
                Beautify(generatedCode, outputFile);
                return;
            }
            if (!string.IsNullOrEmpty(generatedCode))
            {
                File.WriteAllText(outputFile, generatedCode, Encoding.UTF8);
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
                CreateNoWindow = false,
                Arguments = string.Format("-c {0} -o {1} -l java", confingFileName, outputFile)
            };
            proc.Start();
            proc.StandardInput.WriteLine(code);
            proc.Close();
        }

        private JavaSyntaxTree GetSyntaxTree(string inputFile)
        {
            var csSyntaxTree = SyntaxFactory.ParseSyntaxTree(File.ReadAllText(inputFile));
            var semanticModel = SemanticModelBuilder.Build(csSyntaxTree);
            var moduleTranslator = new ModuleTranslator(csSyntaxTree, semanticModel, extensionPoints.ToArray());
            return moduleTranslator.Translate();
        }
    }
}
