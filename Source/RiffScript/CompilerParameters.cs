using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiffScript
{
    public class CompilerParameters
    {
        public List<string> ReferencedAssemblies
        {
            get;
            private set;
        }

        public CompilerParameters()
        {
            this.ReferencedAssemblies = new List<string>();
        }
    }
}
