using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RiffScript;

namespace RiffScriptDemo
{
	class Program
	{
		public static void Main(string[] args)
		{
			ScriptCompiler compiler = new ScriptCompiler();
			var result = compiler.Compile(@"
using System;

public class MyClass
{
	public void DoIt()
	{
	}
}

public void Foo()
{
	Console.WriteLine(""Foo!"");
}

public void Foo(string name)
{
	Console.WriteLine(""Foo with string: {0}"", name);
}

public void Foo(int count)
{
	Console.WriteLine(""Foo with int: {0}"", count);
}

public void Main()
{
	for (int i = 0; i < 10; i++)
	{
		Console.WriteLine(""Hello World! {0}"", i);
	}
}
");
			result.InvokeScriptMethod("Main");
			result.InvokeScriptMethod("Foo");
			result.InvokeScriptMethod("Foo", "Bob");
			result.InvokeScriptMethod("Foo", 42);
			Console.ReadKey();
		}
	}
}
