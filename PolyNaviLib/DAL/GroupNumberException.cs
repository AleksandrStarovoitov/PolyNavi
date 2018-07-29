using System;

namespace PolyNaviLib.DAL
{
    [Serializable]
    public class GroupNumberException : Exception
    {
        public GroupNumberException() { }
        public GroupNumberException(string message) : base(message) { }
        public GroupNumberException(string message, Exception inner) : base(message, inner) { }
        protected GroupNumberException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }
}
