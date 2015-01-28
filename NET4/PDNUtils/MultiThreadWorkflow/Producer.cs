using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace PDNUtils.MultiThreadWorkflow
{
    class Producer<T> : ProducerConsumerBase<T>
    {
        private readonly IItemsProvider<T> itemsProvider;

        public Producer(BlockingCollection<T> queue, CancellationToken cancel, IItemsProvider<T> itemsProvider)
            : base(queue, cancel)
        {
            this.itemsProvider = itemsProvider;
        }

        // start producing data
        public override void Start()
        {
            Thread t = new Thread(ProduceData);
            t.IsBackground = true;
            t.Start();
        }

        protected override string Originator { get { return "Producer: "; } }

        private void ProduceData()
        {
            Message("starting producing data");

            try
            {
                IEnumerable<T> enumerable = itemsProvider.GetItems(cancel);

                foreach (T item in enumerable)
                {
                    cancel.ThrowIfCancellationRequested();
                    pauseHandle.WaitOne();
                    this.queue.Add(item, cancel);
                    Message("added " + item);
                }

                result = OperationResult.Ok;
            }
            catch (OperationCanceledException)
            {
                Message("cancelled");
                result = OperationResult.Canceled;
            }
            finally
            {
                queue.CompleteAdding();
                finishHandle.Set();
                Message("adding completed");
            }
        }
    }
}