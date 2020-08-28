using System;
using System.Runtime.Serialization;

namespace Polynavi.Common.Exceptions
{
    [Serializable]
    public class NoTeacherIdException : Exception
    {
        public NoTeacherIdException()
        {
        }

        public NoTeacherIdException(string message) : base(message)
        {
        }

        public NoTeacherIdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoTeacherIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
