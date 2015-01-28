using System;

namespace LightIndexer.Indexing
{
    public class IndexerResult
    {
        private bool succeeded;
        private string message;
        private Exception exception;

        public IndexerResult(bool succeeded, string message = null, Exception exception = null)
        {
            this.succeeded = succeeded;
            this.message = message;
            this.exception = exception;
        }

        public bool Succeeded { get { return succeeded; } }

        public string Message { get { return message; } }

        public Exception Exception { get { return exception; } }
    }
}