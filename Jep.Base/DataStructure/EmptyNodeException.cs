using System;
using System.Collections.Generic;
using System.Text;

namespace Jep.Base.DataStructure
{
    /// <summary>
    /// 空节点异常
    /// </summary>
    public class EmptyNodeException : Exception
    {
        public EmptyNodeException()
        {
        }

        public EmptyNodeException(string message)
            : base(message)
        {
        }
    }
}
