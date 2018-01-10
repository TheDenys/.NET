using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CompressLib.Exceptions;

namespace CompressLib
{
    /// <summary>
    /// Provides facility for 2 staged items processing: processing them on multiple threads and then sequential post-processing.
    /// Items are consumed from initial collection and being processed by threads count defined by <code>processingThreadsCount</code>.
    /// Processed items get into post-processing queue and can be fetched in the same order as they were in the initial items collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    sealed class ItemsProcessor<T> : IDisposable
    {
        readonly Action<T> _processItemAction;
        readonly Action<T> _postProcessItemAction;
        readonly int _processingThreadsCount;// same as batch size
        // initial state is signalled to let first batch get processed
        readonly ManualResetEvent _finishedBatchEvent = new ManualResetEvent(true);
        readonly object _locker = new object();
        readonly BlockingQueue<OrderedItem<T>> _queueForProcessing = new BlockingQueue<OrderedItem<T>>();
        readonly OrderedBlockingQueue<T> _queueForPostProcessing = new OrderedBlockingQueue<T>();
        readonly IEnumerable<T> _items;
        readonly IList<Exception> _processingExceptions = new List<Exception>();
        Exception _postProcessingException;

        bool _disposed;
        int _offset;
        int _pendingItems;
        int _succeededThreads;

        public ItemsProcessor(ICollection<T> items, Action<T> processItemAction, Action<T> postProcessItemAction, int processingThreadsCount = 1)
        {
            if ((_items = items) == null) throw new ArgumentNullException(nameof(items));
            if ((_processItemAction = processItemAction) == null) throw new ArgumentNullException(nameof(processItemAction));
            if ((_postProcessItemAction = postProcessItemAction) == null) throw new ArgumentNullException(nameof(postProcessItemAction));
            if (processingThreadsCount <= 0) throw new ArgumentOutOfRangeException(nameof(processingThreadsCount));
            _processingThreadsCount = processingThreadsCount;
        }

        /// <summary>
        /// Does processing and post-processing of items in a multi-threaded manner
        /// </summary>
        public void ProcessItems()
        {
            // create and start processing threads
            var processingThreads = Enumerable.Range(1, _processingThreadsCount).Select(i =>
            {
                var t = new Thread(ProcessItemsAndAddToPostProcessingQueue)
                {
                    Name = $"Items processing thread #{i}",
                    IsBackground = true,
                };

                t.Start();
                return t;
            })
            .ToArray();

            var postProcessingThread = new Thread(PostProcessItems)
            {
                Name = "Post processing thread",
                IsBackground = true,
            };
            postProcessingThread.Start();

            // produce items in batches and add them to processing queue
            // doing bathes protects us from memory hogging
            List<OrderedItem<T>> batch;
            long order = 0;
            while ((batch = _items.Skip(_offset).Take(_processingThreadsCount).Select((item) => new OrderedItem<T>(order++, item)).ToList()).Count > 0)
            {
                _finishedBatchEvent.WaitOne();

                lock (_locker)
                {
                    _pendingItems = batch.Count;
                    _finishedBatchEvent.Reset();
                }

                _queueForProcessing.Enqueue(batch);
                _offset += _processingThreadsCount;
            }

            // finished adding items for processing
            _queueForProcessing.FinishAdding();

            foreach (var thread in processingThreads)
            {
                thread.Join();
            }

            // finished adding items for post-processing
            _queueForPostProcessing.FinishAdding();

            // wait for post processing actions to finish
            postProcessingThread.Join();

            if (_processingExceptions.Count > 0)
            {
                throw new CompressLibCompoundException("(De)compressing phase failed. See inner exceptions details.", _processingExceptions);
            }
            else if (_postProcessingException != null)
            {
                throw new CompressLibException("Can't save results.", _postProcessingException);
            }

            if (_succeededThreads != _processingThreadsCount) throw new InvalidOperationException($"Expected {_processingThreadsCount} succeded operations but was {_succeededThreads}. Try re-run the operation.");
        }

        // takes items from queue for processing
        // processes item
        // sends item to post-processing
        void ProcessItemsAndAddToPostProcessingQueue()
        {
            try
            {
                OrderedItem<T> item;
                while (_queueForProcessing.TryDequeue(out item))
                {
                    _processItemAction(item.Value);
                    _queueForPostProcessing.Enqueue(item.Order, item.Value);
                    FinishedItemProcessing();
                }

                Interlocked.Increment(ref _succeededThreads);
            }
            catch (Exception ex)
            {
                // it's no worth to continue if exception happened - data may be corrupted
                _queueForProcessing.FinishAdding();
                _queueForPostProcessing.FinishAdding();
                FinishedItemProcessing();

                lock (_processingExceptions)
                {
                    _processingExceptions.Add(ex);
                }
            }
        }

        // waits for items in post-processing queue and post-processes them in order they are coming
        void PostProcessItems()
        {
            try
            {
                T item;
                while (_queueForPostProcessing.TryDequeue(out item))
                {
                    _postProcessItemAction(item);
                }
            }
            catch (Exception ex)
            {
                // it's no worth to continue if exception happened - data may be corrupted
                _queueForProcessing.FinishAdding();
                _queueForPostProcessing.FinishAdding();
                FinishedItemProcessing();

                _postProcessingException = ex;
            }
        }

        void FinishedItemProcessing()
        {
            lock (_locker)
            {
                if (Interlocked.Decrement(ref _pendingItems) == 0) _finishedBatchEvent.Set();// notify about batch completing
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _queueForProcessing?.Dispose();
            _queueForPostProcessing?.Dispose();
            ((IDisposable)_finishedBatchEvent)?.Dispose();
        }
    }
}