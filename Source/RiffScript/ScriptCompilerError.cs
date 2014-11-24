using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiffScript
{
	public class ScriptCompilerError
	{
		public int Line
		{
			get;
			private set;
		}

		public int Column
		{
			get;
			private set;
		}

		public bool IsWarning
		{
			get;
			private set;
		}

		public string ErrorNumber
		{
			get;
			private set;
		}

		public string ErrorText
		{
			get;
			private set;
		}

		internal ScriptCompilerError(int line, int column, bool isWarning, string errorNumber, string errorText)
		{
			this.Line = line;
			this.Column = column;
			this.IsWarning = IsWarning;
			this.ErrorNumber = errorNumber;
			this.ErrorText = errorText;
		}
	}
}
