using System;
using System.Runtime.Serialization;

namespace Polynavi.Common.Exceptions
{
    [Serializable]
    public class NoGroupIdException : Exception
    {
        public NoGroupIdException()
        {
        }

        public NoGroupIdException(string message) : base(message)
        {
        }

        public NoGroupIdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoGroupIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
