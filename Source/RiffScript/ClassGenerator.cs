using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RiffScript
{
    public class ClassGenerator
    {
        private const string Namespace = "RiffScript.CompiledScripts";
                
        private static string LineDirective(int line)
        {
            return "#line " + line + Environment.NewLine;
        }

        public ClassGeneratorResult Generate(string className, Stream input)
        {
            List<string> lines = new List<string>();
            List<string> references = new List<string>();
            List<string> usings = new List<string>();
            List<string> fields = new List<string>();
            List<string> methods = new List<string>();
            List<string> types = new List<string>();

            StreamReader sr = new StreamReader(input);
            while (!sr.EndOfStream)
                lines.Add(sr.ReadLine());

            int i = 0;
            while (lines[i].StartsWith("#reference "))
            {
                string reference = lines[i].Substring("#reference ".Length).Trim();
                reference = reference.Trim('"');

                references.Add(reference);

                i++;
            }

            string script = string.Join(Environment.NewLine, lines);

            var syntaxTree = CSharpSyntaxTree.ParseText(script);

            var directive = syntaxTree.GetRoot().GetFirstDirective();

            foreach (var node in syntaxTree.GetRoot().ChildNodes())
            {
                var line = node.GetLocation().GetLineSpan().StartLinePosition.Line;

                if (node is UsingDirectiveSyntax)
                {
                    usings.Add(LineDirective(line) + node.ToString());
                }
                else if (node is FieldDeclarationSyntax)
                {
                    fields.Add(LineDirective(line) + node.ToString());
                }
                else if (node is MethodDeclarationSyntax)
                {
                    methods.Add(LineDirective(line) + node.ToString());
                }
                else if (node is TypeDeclarationSyntax)
                {
                    types.Add(LineDirective(line) + node.ToString());
                }
                else
                {
                    throw new NotImplementedException("Unhandled node type " + node.GetType());
                }
            }

            StringBuilder source = new StringBuilder();

            source.AppendLine(string.Join(Environment.NewLine, usings.Distinct()));
            source.AppendLine();
            source.AppendLine(string.Format("namespace {0}", Namespace));
            source.AppendLine("{");
            source.AppendLine("\tpublic class " + className + " : Script");
            source.AppendLine("\t{");
            source.AppendLine(string.Join(Environment.NewLine, types));
            source.AppendLine();
            source.AppendLine(string.Join(Environment.NewLine, fields));
            source.AppendLine();
            source.AppendLine("\t\tpublic " + className + "(ScriptContext context)");
            source.AppendLine("\t\t\t: base(context)");
            source.AppendLine("\t\t{");
            source.AppendLine("\t\t}");
            source.AppendLine();
            source.AppendLine(string.Join(Environment.NewLine + Environment.NewLine, methods));
            source.AppendLine("\t}");
            source.AppendLine("}");

            return new ClassGeneratorResult(
                references.ToArray(),
                Namespace + "." + className,
                source.ToString()
            );
        }
    }
}
