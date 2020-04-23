using System;
using System.Runtime.Serialization;

namespace Polynavi.Common.Exceptions
{
    public class SameRoomsSelectedException : Exception
    {
        public SameRoomsSelectedException()
        {
        }

        public SameRoomsSelectedException(string message) : base(message)
        {
        }

        public SameRoomsSelectedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SameRoomsSelectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
