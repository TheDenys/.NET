using System;
using System.Collections.Concurrent;
using System.Threading;
using PDNUtils.Help;

namespace PDNUtils.MultiThreadWorkflow
{
    abstract class ProducerConsumerBase<T> : IDisposable
    {
        public enum OperationResult { Undefined = 0, Ok = 1, Canceled = 2 }

        protected OperationResult result;

        public OperationResult Result
        {
            get { return result; }
        }

        protected readonly BlockingCollection<T> queue;

        protected readonly CancellationToken cancel;

        protected readonly object sync = new object();

        protected ManualResetEvent pauseHandle = new ManualResetEvent(true);
        protected ManualResetEventSlim finishHandle = new ManualResetEventSlim(false);


        protected ProducerConsumerBase(BlockingCollection<T> queue, CancellationToken cancel)
        {
            cancel.Register(() =>
                                {
                                    lock (sync)
                                    {
                                        pauseHandle.Set();
                                        finishHandle.Set();
                                    }
                                });
            this.queue = queue;
            this.cancel = cancel;
        }

        // start producing/consuming data
        public abstract void Start();

        protected abstract string Originator { get; }

        public void Pause()
        {
            Message("pausing");
            pauseHandle.Reset();
        }

        public void Resume()
        {
            Message("resuming");
            pauseHandle.Set();
        }

        public void Wait()
        {
            //lock (sync)
            finishHandle.Wait(cancel);
        }

        protected void Message(string s)
        {
            //ConsolePrint.print(Originator + s);
        }

        public void Dispose()
        {
            lock (sync)
            {
                pauseHandle.Dispose();
                finishHandle.Dispose();
            }
        }
    }
}