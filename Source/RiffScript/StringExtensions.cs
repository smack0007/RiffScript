using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiffScript
{
	internal static class StringExtensions
	{
		public static string Indent(this string input, int level)
		{
			string indent = new string('\t', level);
			return indent + input.Replace(Environment.NewLine, Environment.NewLine + indent);
		}
	}
}
