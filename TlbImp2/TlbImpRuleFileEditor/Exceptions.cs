using System;
using System.Collections.Generic;
using System.Text;

namespace TlbImpRuleFileEditor
{
    /// <summary>
    /// The resource cannot be found
    /// </summary>
    public class TlbImpResourceNotFoundException : ApplicationException
    {
        public TlbImpResourceNotFoundException(string msg)
            : base(msg)
        {
        }
    }

    [Serializable]
    public class ParseDataFailedException : ApplicationException
    {
    }

    [Serializable]
    public class ParseDataTypeUnsupportedException : ApplicationException
    {
    }

}
