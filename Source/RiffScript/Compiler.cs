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
            var compilerParams = new System.CodeDom.Compiler.CompilerParameters()
			{
				GenerateInMemory = true
			};

			compilerParams.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Core.dll");
            compilerParams.ReferencedAssemblies.Add("System.Data.dll");
            compilerParams.ReferencedAssemblies.Add("System.Data.DataSetExtensions.dll");
            compilerParams.ReferencedAssemblies.Add("System.Net.dll");
            compilerParams.ReferencedAssemblies.Add("System.Xml.dll");
            compilerParams.ReferencedAssemblies.Add("System.Xml.Linq.dll");

            foreach (var reference in parameters.ReferencedAssemblies)
            {
                string name = reference;

                if (!name.EndsWith(".dll"))
                    name = name + ".dll";

                compilerParams.ReferencedAssemblies.Add(name);
            }

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
				throw new CompilerException(
					"Failed to compile script.",
                    compilerResults.Errors
                        .Cast<System.CodeDom.Compiler.CompilerError>()
                        .Select(x => new CompilerError(x.Line, x.Column, x.IsWarning, x.ErrorNumber, x.ErrorText))
                        .ToArray()
				);
			}

			var script = (Script)Activator.CreateInstance(compilerResults.CompiledAssembly.GetType(classGeneratorResult.FullTypeName), new object[] { new ScriptContext() });
			return script;
		}
	}
}
