using System;
using System.Runtime.Serialization;

namespace CompressLib.Exceptions
{
    /// <summary>
    /// We don't want to (de)compress an empty file
    /// </summary>
    public class EmptyFileException:CompressLibException
    {
        public EmptyFileException()
        {
        }

        public EmptyFileException(string message) : base(message)
        {
        }

        public EmptyFileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EmptyFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}