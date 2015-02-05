using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiffScript
{
    public class ScriptMethodResult
    {
        public object ReturnValue
        {
            get;
            internal set;
        }

        public Exception Exception
        {
            get;
            internal set;
        }

        internal ScriptMethodResult()
        {
        }
    }
}
