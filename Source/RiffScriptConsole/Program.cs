using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiffScript;

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

			ScriptCompiler compiler = new ScriptCompiler();

			Script script = null;

			try
			{
				script = compiler.Compile(File.OpenRead(args[0]));
			}
			catch (ScriptCompilerException ex)
			{
				Console.Error.WriteLine("Errors while compiling script:");

				foreach (var error in ex.Errors)
				{
					Console.Error.WriteLine("({0},{1}): error {2}: {3}", error.Line, error.Column, error.ErrorNumber, error.ErrorText);
				}

				return 1;
			}

			object result = null;

			if (script.ScriptMethodExists("Main", typeof(string[])))
			{
				string[] scriptArgs = args.Skip(1).ToArray();
				result = script.InvokeScriptMethod("Main", scriptArgs);
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

			if (result is int)
			{
				return (int)result;
			}

			return 0;
		}
	}
}
