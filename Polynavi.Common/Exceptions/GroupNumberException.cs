using System;
using System.Runtime.Serialization;

namespace Polynavi.Common.Exceptions
{
    [Serializable]
    public class GroupNumberException : Exception
    {
        public GroupNumberException()
        {
        }

        public GroupNumberException(string message) : base(message)
        {
        }

        public GroupNumberException(string message, Exception inner) : base(message, inner)
        {
        }

        protected GroupNumberException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
