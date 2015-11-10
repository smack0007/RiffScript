using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RiffScript
{
	public class Compiler
	{
		ClassGenerator generator;
		int scriptCount;

		public Compiler()
		{
			this.generator = new ClassGenerator();
		}

		private static Stream StringToStream(string input)
		{
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(input);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}

		public Script Compile(string code, CompilerParameters parameters)
		{
			using (Stream stream = StringToStream(code))
			{
				return this.Compile(stream, parameters);
			}
		}

		public Script Compile(Stream stream, CompilerParameters parameters)
		{
			this.scriptCount++;
			return this.Compile(this.generator.Generate("Script" + this.scriptCount, stream), parameters);
		}

		private Script Compile(ClassGeneratorResult classGeneratorResult, CompilerParameters parameters)
		{
            string gacPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

            List<MetadataReference> references = new List<MetadataReference>()
            {
                MetadataReference.CreateFromFile(Path.Combine(gacPath, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(gacPath, "Microsoft.CSharp.dll")),
                MetadataReference.CreateFromFile(Path.Combine(gacPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(gacPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(gacPath, "System.Data.dll")),
                MetadataReference.CreateFromFile(Path.Combine(gacPath, "System.Data.DataSetExtensions.dll")),
                MetadataReference.CreateFromFile(Path.Combine(gacPath, "System.Xml.dll")),
                MetadataReference.CreateFromFile(Path.Combine(gacPath, "System.Xml.Linq.dll")),
                MetadataReference.CreateFromFile(typeof(Script).Assembly.Location)
            };

            foreach (var reference in parameters.ReferencedAssemblies.Concat(classGeneratorResult.References))
            {
                string path = reference;

                if (!path.EndsWith(".dll"))
                    path = path + ".dll";

                if (!File.Exists(path))
                    path = Path.Combine(gacPath, path);

                references.Add(MetadataReference.CreateFromFile(Path.Combine(gacPath, path)));
            }

            var compilation = CSharpCompilation.Create(classGeneratorResult.FullTypeName + ".dll")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .WithReferences(references)
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(classGeneratorResult.Source));

            Assembly assembly = null;

            using (MemoryStream ms = new MemoryStream(1024))
            {
                var emitResult = compilation.Emit(ms);

                if (!emitResult.Success)
                {
                    throw new CompilerException(
                        "Failed to compile script.",
                        emitResult
                            .Diagnostics
                            .Select(x => new CompilerError(
                                x.Location.GetLineSpan().StartLinePosition.Line,
                                x.Location.GetLineSpan().StartLinePosition.Character,
                                x.Severity == DiagnosticSeverity.Error,
                                x.Id,
                                x.GetMessage()
                            ))
                            .ToArray()
                    );
                }

                assembly = Assembly.Load(ms.ToArray());
            }
                        
			return (Script)Activator.CreateInstance(assembly.GetType(classGeneratorResult.FullTypeName), new object[] { new ScriptContext() });
		}
	}
}
