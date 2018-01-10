using System;
using System.Collections.Generic;
using System.Threading;

namespace CompressLib
{
    // This class lets arbitrary amount of producers to add items unordered and the single consumer read them ordered
    sealed class OrderedBlockingQueue<T> : IDisposable
    {
        readonly object _locker = new object();

        private long _nextItemOrder;

        IDictionary<long, T> _itemsDictionary = new Dictionary<long, T>();

        private bool disposed;
        ManualResetEvent nextItemAddedEvent = new ManualResetEvent(false);

        private volatile bool _finishedAdding;

        public void Enqueue(long order, T item)
        {
            lock (_locker)
            {
                _itemsDictionary.Add(order, item);

                if (_itemsDictionary.ContainsKey(_nextItemOrder))
                {
                    // signal next item is present
                    nextItemAddedEvent.Set();
                }
            }
        }

        // returns the next item according to order or waits till such item is added
        public bool TryDequeue(out T item)
        {
            item = default(T);

            while (ShouldTryDequeue())
            {
                nextItemAddedEvent.WaitOne();

                lock (_locker)
                {
                    T localChunk;
                    if (_itemsDictionary.TryGetValue(_nextItemOrder, out localChunk))
                    {
                        _itemsDictionary.Remove(_nextItemOrder);
                        Interlocked.Increment(ref _nextItemOrder);
                        item = localChunk;
                        return true;
                    }
                    else if (_itemsDictionary.Count == 0 && _finishedAdding)
                    {
                        return false;
                    }

                    if (!_finishedAdding && !_itemsDictionary.ContainsKey(_nextItemOrder)) nextItemAddedEvent.Reset();
                }
            }

            return false;
        }

        // we should call finish adding to unblock consuming threads after we finished producing items
        public void FinishAdding()
        {
            lock (_locker)
            {
                _finishedAdding = true;
                nextItemAddedEvent.Set();
            }
        }

        public void Dispose()
        {
            if (disposed) return;
            ((IDisposable)nextItemAddedEvent)?.Dispose();
        }

        bool ShouldTryDequeue()
        {
            lock (_locker)
            {
                return !_finishedAdding || _itemsDictionary.Count != 0;
            }
        }
    }
}