using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiffScript
{
	public class ScriptCompilerException : Exception
	{
		public IReadOnlyCollection<ScriptCompilerError> Errors
		{
			get;
			private set;
		}

		internal ScriptCompilerException(string message, ScriptCompilerError[] errors)
			: base(message)
		{
			this.Errors = errors;
		}
	}
}
