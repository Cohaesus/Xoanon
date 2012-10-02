//-----------------------------------------------------------------------
// <copyright file="ScriptFailureException.cs" company="Cohaesus Project Ltd">
//     Copyright (c) Cohaesus Project Ltd. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DbCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class ScriptFailureException : Exception
    {
        public ScriptFailureException() { }
        public ScriptFailureException(string message) : base() { }
    }
}
