﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiffScript
{
	public class ScriptException : Exception
	{
		public ScriptException(string message)
			: base(message)
		{
		}
	}
}
