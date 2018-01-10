using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CompressLib.Exceptions
{
    /// <summary>
    /// Basically an AggregateException
    /// </summary>
    public class CompressLibCompoundException : Exception
    {
        public IList<Exception> InnerExceptions { get; }

        public CompressLibCompoundException(string message, IList<Exception> innerExceptions) : base(message)
        {
            InnerExceptions = new ReadOnlyCollection<Exception>(innerExceptions);
        }
    }
}