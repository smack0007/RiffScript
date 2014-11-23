using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiffScript
{
	public class ClassGeneratorResult
	{		
		public string[] References
		{
			get;
			private set;
		}

		public string FullTypeName
		{
			get;
			private set;
		}

		public string Source
		{
			get;
			private set;
		}

		internal ClassGeneratorResult(string[] references, string fullTypeName, string source)
		{
			this.References = references;
			this.FullTypeName = fullTypeName;
			this.Source = source;
		}
	}
}
