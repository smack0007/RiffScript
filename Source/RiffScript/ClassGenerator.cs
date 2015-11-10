using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;

namespace RiffScript
{
    public class ClassGenerator
    {
        private const string Namespace = "RiffScript.CompiledScripts";

        private readonly string[] StandardUsings = new string[]
        {
            "using System;",
            "using System.Collections.Generic;",
            "using System.Linq;",
            "using System.Text;",
            "using System.Threading;",
            "using System.Threading.Tasks;"
        };

        private static string LineDirective(int line)
        {
            return "#line " + line + Environment.NewLine;
        }

        public ClassGeneratorResult Generate(string className, Stream input)
        {
            List<string> lines = new List<string>();
            List<string> references = new List<string>();
            List<string> usings = new List<string>(StandardUsings);
            List<string> fields = new List<string>();
            List<string> methods = new List<string>();
            List<string> types = new List<string>();

            StreamReader sr = new StreamReader(input);
            while (!sr.EndOfStream)
                lines.Add(sr.ReadLine());

            if (lines.Count > 0)
            {
                int i = 0;
                while (lines[i].StartsWith("#reference "))
                {
                    string reference = lines[i].Substring("#reference ".Length).Trim();
                    reference = reference.Trim('"');

                    references.Add(reference);

                    i++;
                }

                while (i < lines.Count && string.IsNullOrWhiteSpace(lines[i]))
                    i++;

                while (i < lines.Count && lines[i].StartsWith("using ") && !lines[i].Contains("("))
                {
                    usings.Add(LineDirective(i) + lines[i].Trim());
                    i++;
                }

                string script = string.Join(Environment.NewLine, lines.Skip(i));

                CSharpParser parser = new CSharpParser();
                var members = parser.ParseTypeMembers(script);

                foreach (var member in members)
                {
                    if (member is FieldDeclaration)
                    {
                        var field = (FieldDeclaration)member;

                        fields.Add(field.ToString().Trim().Indent(2));
                    }
                    else if (member is MethodDeclaration)
                    {
                        var method = (MethodDeclaration)member;

                        methods.Add(method.ToString().Trim().Indent(2));
                    }
                    else if (member is TypeDeclaration)
                    {
                        var type = (TypeDeclaration)member;

                        types.Add(type.ToString().Trim().Indent(2));
                    }
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
