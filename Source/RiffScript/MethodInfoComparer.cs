using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RiffScript
{
	internal class MethodInfoComparer : IEqualityComparer<MethodInfo>
	{		
		public bool Equals(MethodInfo x, MethodInfo y)
		{
			if (x.Name != y.Name)
				return false;

			if (x.ReturnType != y.ReturnType)
				return false;

			var xParameters = x.GetParameters();
			var yParameters = y.GetParameters();

			if (xParameters.Length != yParameters.Length)
				return false;

			for (int i = 0; i < xParameters.Length; i++)
			{
				if (xParameters[i].ParameterType != yParameters[i].ParameterType)
					return false;
			}

			return true;
		}

		public bool Equals(MethodInfo x, string name, object[] parameters)
		{
			if (x.Name != name)
				return false;
						
			var xParameters = x.GetParameters();

			if (xParameters.Length != parameters.Length)
				return false;

			for (int i = 0; i < xParameters.Length; i++)
			{
				if (xParameters[i].ParameterType != parameters[i].GetType())
					return false;
			}

			return true;
		}

		public int GetHashCode(MethodInfo obj)
		{
			return obj.Name.GetHashCode() ^ obj.GetParameters().Length;
		}
	}
}
