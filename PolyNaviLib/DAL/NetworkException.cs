﻿using System;
using System.Runtime.Serialization;

namespace PolyNaviLib.DAL
{
    [Serializable]
    public class NetworkException : Exception
    {
        public NetworkException() { }
        public NetworkException(string message) : base(message) { }
        public NetworkException(string message, Exception inner) : base(message, inner) { }
        protected NetworkException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }
}
