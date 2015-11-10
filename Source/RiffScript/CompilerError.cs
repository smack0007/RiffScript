using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiffScript
{
	public class CompilerError
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

		internal CompilerError(int line, int column, bool isWarning, string errorNumber, string errorText)
		{
			this.Line = line;
			this.Column = column;
			this.IsWarning = IsWarning;
			this.ErrorNumber = errorNumber;
			this.ErrorText = errorText;
		}

        public override string ToString()
        {
            return string.Format(
                "({0}, {1}) {2} {3} {4}",
                this.Line,
                this.Column,
                this.IsWarning ? "Warning" : "Error",
                this.ErrorNumber,
                this.ErrorText);
        }
    }
}
