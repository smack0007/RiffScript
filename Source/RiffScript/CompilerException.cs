using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiffScript
{
	public class CompilerException : Exception
	{
		public IEnumerable<CompilerError> Errors
		{
			get;
			private set;
		}

		internal CompilerException(string message, CompilerError[] errors)
			: base(message)
		{
			this.Errors = errors;
		}
	}
}
