using System;
using System.IO;
using log4net;
using Lucene.Net.Documents;

namespace LightIndexer.Indexing
{
    /// <summary>
    /// Proxy class that provides ability to release unmanaged resource used by FileStream.
    /// Class has a destructor so even if nobody calls Close or Dispose explicitly finalizer probably will be called by GC and releaze.
    /// </summary>
    public sealed class IndexerDocument : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Document doc;

        private Stream fs;

        public IndexerDocument(Document doc, Stream fs)
        {
            this.doc = doc;
            this.fs = fs;
        }

        public Document Doc
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("document was disposed already");
                }

                return doc;
            }
        }

        public void Close()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("document was disposed already");
            }

            Dispose();
        }

        private void Release()
        {
            if (fs != null)
            {
                try
                {
                    fs.Close();
                    fs = null;
                }
                catch (System.Exception e)
                {
                    log.Error(e);
                }
            }
        }

        private bool disposed = false;

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            // if we had Finalize we would need call this
            //GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.doc = null;
                }

                Release();
                disposed = true;
            }
        }

        // Finalize is expensive in .net, so we dont' implement it
        // user can use Close or Dispose, it should be enough
        //~IndexerDocument()
        //{
        //    // Simply call Dispose(false).
        //    Dispose(false);
        //}
    }
}