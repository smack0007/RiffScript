using System;
using RiffScript;

namespace RiffScriptDemo
{
    class Program
	{
		public static void Main(string[] args)
		{
            string code = @"
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
";

			Compiler compiler = new Compiler();
			var result = compiler.Compile(code, new CompilerParameters());

			result.InvokeScriptMethod("Main");
			result.InvokeScriptMethod("Foo");
			result.InvokeScriptMethod("Foo", "Bob");
			result.InvokeScriptMethod("Foo", 42);
			Console.ReadKey();
		}
	}
}
