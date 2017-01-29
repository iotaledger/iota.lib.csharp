using System;
using System.Runtime.Serialization;

namespace Iota.Lib.CSharp.Api.Exception
{
    [Serializable]
    internal class InvalidSignatureException : System.Exception
    {
        public InvalidSignatureException()
        {
        }

        public InvalidSignatureException(string message) : base(message)
        {
        }

        public InvalidSignatureException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidSignatureException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}