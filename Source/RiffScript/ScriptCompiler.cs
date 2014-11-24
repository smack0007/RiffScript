using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RiffScript
{
	public class ScriptCompiler
	{
		ClassGenerator generator;
		int scriptCount;

		public ScriptCompiler()
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

		public Script Compile(string code)
		{
			using (Stream stream = StringToStream(code))
			{
				return this.Compile(stream);
			}
		}

		public Script Compile(Stream stream)
		{
			this.scriptCount++;
			return this.Compile(this.generator.Generate("Script" + this.scriptCount, stream));
		}

		private Script Compile(ClassGeneratorResult classGeneratorResult)
		{
			var compilerParams = new CompilerParameters()
			{
				GenerateInMemory = true
			};

			compilerParams.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

			foreach (var reference in classGeneratorResult.References)
			{
				string name = reference;

				if (!name.EndsWith(".dll"))
					name = name + ".dll";

				compilerParams.ReferencedAssemblies.Add(name);
			}

			var compiler = CodeDomProvider.CreateProvider("CSharp");
			var compilerResults = compiler.CompileAssemblyFromSource(compilerParams, classGeneratorResult.Source);

			if (compilerResults.Errors.Count > 0)
			{
				throw new ScriptCompilerException(
					"Failed to compile script.",
					compilerResults.Errors.Cast<CompilerError>().Select(x => new ScriptCompilerError(x.Line, x.Column, x.IsWarning, x.ErrorNumber, x.ErrorText)).ToArray()
				);
			}

			var script = (Script)Activator.CreateInstance(compilerResults.CompiledAssembly.GetType(classGeneratorResult.FullTypeName), new object[] { new ScriptContext() });
			return script;
		}
	}
}
