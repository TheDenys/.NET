using System;
using System.Runtime.Serialization;

namespace CompressLib.Exceptions
{
    /// <summary>
    /// Base exception for CompressLib
    /// </summary>
    public class CompressLibException : Exception
    {
        public CompressLibException()
        {
        }

        public CompressLibException(string message) : base(message)
        {
        }

        public CompressLibException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CompressLibException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}