using System;
using System.Collections.Generic;
using System.Text;

namespace PolyNaviLib.DAL
{

	[Serializable]
	public class NetworkException : Exception
	{
		public NetworkException() { }
		public NetworkException(string message) : base(message) { }
		public NetworkException(string message, Exception inner) : base(message, inner) { }
		protected NetworkException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
