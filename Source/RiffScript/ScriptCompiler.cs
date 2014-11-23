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
			this.scriptCount++;

			using (Stream stream = StringToStream(code))
			{
				return this.Compile(this.generator.Generate("Script" + this.scriptCount, stream));
			}
		}

		public Script Compile(ClassGeneratorResult classGeneratorResult)
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
				StringBuilder errors = new StringBuilder();
				errors.AppendLine("Failed to compile script:");
				foreach (CompilerError error in compilerResults.Errors)
				{
					errors.AppendLine(string.Format("({0}, {1}): {2}", error.Line, error.Column, error.ErrorText));
				}

				throw new CompileException(errors.ToString());
			}

			var script = (Script)Activator.CreateInstance(compilerResults.CompiledAssembly.GetType(classGeneratorResult.FullTypeName), new object[] { new ScriptContext() });
			return script;
		}
	}
}
