using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbCompiler
{
    [Serializable]
    public class ScriptFailureException : Exception
    {
        public ScriptFailureException() { }
        public ScriptFailureException(string message) : base() { }
    }
}
