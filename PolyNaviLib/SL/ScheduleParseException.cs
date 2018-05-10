using System;
using System.Collections.Generic;
using System.Text;

namespace PolyNaviLib.SL
{

	[Serializable]
	public class ScheduleParseException : Exception
	{
		public ScheduleParseException() { }
		public ScheduleParseException(string message) : base(message) { }
		public ScheduleParseException(string message, Exception inner) : base(message, inner) { }
		protected ScheduleParseException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
