using System;
using System.Runtime.Serialization;

namespace Graph
{
    [Serializable]
    public class GraphRoutingException : Exception
    {
        public GraphRoutingException() { }
        public GraphRoutingException(string message) : base(message) { }
        public GraphRoutingException(string message, Exception inner) : base(message, inner) { }
        protected GraphRoutingException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }
}
