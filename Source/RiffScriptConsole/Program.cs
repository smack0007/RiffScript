using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiffScript;
using System.Reflection;

namespace RiffScriptConsole
{
	class Program
	{
		public static int Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.Error.WriteLine("Please provide a script file name.");
				return 1;
			}

			if (!File.Exists(args[0]))
			{
				Console.Error.WriteLine("Script {0} does not exist.", args[0]);
				return 1;
			}

            string[] scriptArgs = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--")
                {
                    scriptArgs = args.Skip(i + 1).ToArray();
                }
            }

            if (scriptArgs == null)
                scriptArgs = new string[0];

			Compiler compiler = new Compiler();

			Script script = null;

            CompilerParameters parameters = new CompilerParameters();

            string scriptDirectory = Path.GetDirectoryName(Path.GetFullPath(args[0]));
            Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();

            if (Directory.Exists(Path.Combine(scriptDirectory, "references")))
            {
                foreach (string dllFile in Directory.GetFiles(Path.Combine(scriptDirectory, "references"), "*.dll").Select(x => Path.GetFullPath(x)))
                {
                    parameters.ReferencedAssemblies.Add(dllFile);

                    Assembly assembly = Assembly.LoadFile(dllFile);
                    assemblies[assembly.FullName] = assembly;
                }
            }

            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                if (assemblies.ContainsKey(e.Name))
                {
                    return assemblies[e.Name];
                }

                return null;
            };

			try
			{
				script = compiler.Compile(File.OpenRead(args[0]), parameters);
			}
			catch (CompilerException ex)
			{
                Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine("Errors while compiling script:");
				foreach (var error in ex.Errors)
				{
					Console.Error.WriteLine(error);
				}
                Console.ResetColor();

				return 1;
			}

            Directory.SetCurrentDirectory(scriptDirectory);

			ScriptMethodResult result = null;

			if (script.ScriptMethodExists("Main", typeof(string[])))
			{
				result = script.InvokeScriptMethod("Main", new object[] { scriptArgs });
			}
			else if (script.ScriptMethodExists("Main"))
			{
				result = script.InvokeScriptMethod("Main");
			}
			else
			{
				Console.Error.WriteLine("Script contains no public Main method.");
				return 1;
			}

            if (result.Exception != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Exception thrown during script execution:");
                Console.Error.WriteLine(result.Exception);
                Console.ResetColor();
                return 2;
            }

			if (result.ReturnValue is int)
			{
				return (int)result.ReturnValue;
			}

			return 0;
		}
	}
}
