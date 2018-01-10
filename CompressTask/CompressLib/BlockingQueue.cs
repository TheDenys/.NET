using System;
using System.Collections.Generic;
using System.Threading;

namespace CompressLib
{
    // this class let's producers add items and consumers take them via blocking TryDequeue operation
    sealed class BlockingQueue<T> : IDisposable
    {
        // Queue class is not thread safe so we must synchronize dequeueing
        readonly object _dequeueLocker = new object();
        readonly Queue<T> _queue = new Queue<T>();

        readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        readonly ManualResetEvent _hasItemsEvent = new ManualResetEvent(false);
        volatile bool _disposed;
        volatile bool _finishedAdding;

        public void Enqueue(ICollection<T> items)
        {
            try
            {
                _rwLock.EnterWriteLock();// entering exclusive lock before adding items to queue

                foreach (var item in items)
                {
                    _queue.Enqueue(item);
                }

                _hasItemsEvent.Set();
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public bool TryDequeue(out T item)
        {
            item = default(T);
            int queueCount = 1;

            while (ShouldTryDequeue())
            {
                try
                {
                    // waiting for items to appear in queue
                    // wait must be before entering read lock, otherwise deadlock will happen
                    _hasItemsEvent.WaitOne();
                    _rwLock.EnterReadLock();

                    lock (_dequeueLocker)
                    {
                        if (_queue.Count > 0)
                        {
                            item = _queue.Dequeue();

                            if (_queue.Count == 0 && !_finishedAdding)
                            {
                                // tell other readers to wait for more items if not finished adding
                                _hasItemsEvent.Reset();
                            }

                            return true;
                        }
                        else
                        {
                            // tell other readers to wait for more items if not finished adding
                            if (!_finishedAdding) _hasItemsEvent.Reset();
                        }
                    }
                }
                finally
                {
                    _rwLock.ExitReadLock();
                }
            }

            return false;
        }

        public void FinishAdding()
        {
            try
            {
                _rwLock.EnterWriteLock();// making sure all readers have left the lock
                _finishedAdding = true;
                _hasItemsEvent.Set();// unblocking all waiting readers
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _rwLock?.Dispose();
            ((IDisposable)_hasItemsEvent)?.Dispose();
            _disposed = true;
        }

        bool ShouldTryDequeue()
        {
            lock (_dequeueLocker)
            {
                return !_finishedAdding || _queue.Count != 0;
            }
        }
    }
}