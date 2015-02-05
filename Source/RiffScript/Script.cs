using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RiffScript
{
	public abstract class Script
	{
		ScriptContext context;
		MethodInfoComparer methodInfoComparer;

		protected Script(ScriptContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			this.context = context;

			this.methodInfoComparer = new MethodInfoComparer();
		}

		public IEnumerable<MethodInfo> GetScriptMethods()
		{
			return this.GetType().GetMethods().Except(typeof(Script).GetMethods(), this.methodInfoComparer);
		}

		public IEnumerable<string> GetScriptMethodNames()
		{
			return this.GetScriptMethods().Select(x => x.Name);
		}
				
		public bool ScriptMethodExists(string name, params Type[] parameterTypes)
		{
			return this.GetScriptMethods().FirstOrDefault(x => this.methodInfoComparer.Equals(x, name, parameterTypes)) != null;
		}

		public ScriptMethodResult InvokeScriptMethod(string name, params object[] parameters)
		{
			MethodInfo method = this.GetScriptMethods().FirstOrDefault(x => this.methodInfoComparer.Equals(x, name, parameters));

			if (method == null)
				throw new ScriptException(string.Format("Unable to locate method \"{0}\". Either no method with that name exists or unable to match the given parameters.", name));

            ScriptMethodResult result = new ScriptMethodResult();

            try
            {
                result.ReturnValue = method.Invoke(this, parameters);
            }
			catch (TargetInvocationException ex)
            {
                result.Exception = ex.InnerException;
            }

            return result;
		}
	}
}
