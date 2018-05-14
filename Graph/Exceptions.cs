using System;
using System.Collections.Generic;
using System.Text;

namespace Graph
{
	[Serializable]
	public class GraphRoutingException : Exception
	{
		public GraphRoutingException() { }
		public GraphRoutingException(string message) : base(message) { }
		public GraphRoutingException(string message, Exception inner) : base(message, inner) { }
		protected GraphRoutingException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
