using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace PDNUtils.MultiThreadWorkflow
{
    class Consumer<T> : ProducerConsumerBase<T>, IDisposable
    {

        readonly Action<OperationResult> finished;
        //readonly SemaphoreSlim s;
        CountdownEvent cde = new CountdownEvent(1);
        readonly Action<T> processItem;

        public Consumer(BlockingCollection<T> queue, CancellationToken cancel, int maxTaskAmount, Action<OperationResult> finished, Action<T> processItem)
            : base(queue, cancel)
        {
            if (finished == null)
                throw new ArgumentNullException("finished");
            if (cancel == null)
                throw new ArgumentNullException("cancel");
            if (queue == null)
                throw new ArgumentNullException("queue");
            if (processItem == null)
                throw new ArgumentNullException("processItem");
            //this.s = new SemaphoreSlim(maxTaskAmount, maxTaskAmount);
            this.finished = finished;
            this.processItem = processItem;
        }

        // start consuming 
        public override void Start()
        {
            Thread t = new Thread(ConsumeData);
            t.IsBackground = true;
            t.Start();
        }

        protected override string Originator { get { return "Consumer: "; } }

        private void ConsumeData()
        {
            Message("starting consuming data");

            try
            {
                Action<Task> onFinish2 = (t) => { cde.Signal(); };

                try
                {
                    foreach (var item in queue.GetConsumingEnumerable(cancel))
                    {
                        pauseHandle.WaitOne();
                        //s.Wait(cancel);
                        Message("got item: " + item);

                        try
                        {
                            cde.AddCount();
                            var task = Task.Factory.StartNew(ProcessItem, item);
                            //if (cde != null)
                            //    task.ContinueWith(onFinish2);
                        }
                        catch (InvalidCastException ice)
                        {
                            //s.Release();
                            Message(ice.ToString());
                        }
                    }

                    result = OperationResult.Ok;
                }
                catch (OperationCanceledException oce)
                {
                    result = OperationResult.Canceled;
                    Message("canceled " + oce);
                }

                Message("finished consuming");

                cde.Signal();
                cde.Wait(cancel);
                finishHandle.Set();
                finished(Result);
            }
            catch (OperationCanceledException oce)
            {
                result = OperationResult.Canceled;
                Message("canceled " + oce);
            }
            finally
            {
                if (cde != null)
                    cde.Dispose();
            }
        }

        private void ProcessItem(object o)
        {
            try
            {
                var item = (T)o;
                Message("processing item " + item);
                processItem(item);
            }
            catch (Exception e)
            {
                Message(e.ToString());
            }
            finally
            {
                //s.Release();
                //cde.Signal();
            }
        }

        void IDisposable.Dispose()
        {
            try
            {
                base.Dispose();
                //s.Dispose();
                if (cde != null)
                    cde.Dispose();

            }
            catch (Exception e)
            {
                Message("Problem in Dispose: " + e);
            }
        }
    }
}