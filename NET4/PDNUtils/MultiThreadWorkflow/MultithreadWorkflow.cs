using System;
using System.Collections.Concurrent;
using System.Threading;
using PDNUtils.Help;

namespace PDNUtils.MultiThreadWorkflow
{
    // producer -> queue -> consumer
    public sealed class MultithreadWorkflow<T> : IDisposable
    {
        readonly int QueueSize = 2048;
        readonly int MaxTaskAmount = 200;

        readonly CancellationTokenSource cts = new CancellationTokenSource();
        readonly BlockingCollection<T> queue;
        readonly ProducerConsumerBase<T> producer;
        readonly ProducerConsumerBase<T> consumer;

        private State currentState;

        State CurrentState
        {
            get { return currentState; }
            set
            {
                Message("Setting state to " + value);
                currentState = value;
            }
        }

        readonly State pendingState;
        readonly State workingState;
        readonly State pausedState;
        readonly State stoppedState;
        readonly State finishedState;
        readonly State disposedState;

        public MultithreadWorkflow(IItemsProvider<T> itemsProvider, Action<T> processItem)
        {
            //queue = new BlockingCollection<T>(QueueSize);
            queue = new BlockingCollection<T>();
            producer = new Producer<T>(queue, cts.Token, itemsProvider);
            consumer = new Consumer<T>(queue, cts.Token, MaxTaskAmount, (r) => { CurrentState = finishedState; }, processItem);

            //init states
            pendingState = new PendingState(this);
            workingState = new WorkingState(this);
            pausedState = new PausedState(this);
            stoppedState = new StoppedState(this);
            finishedState = new FinishedState(this);
            disposedState = new DisposedState(this);
            currentState = pendingState;
        }

        public void Start()
        {
            CurrentState.Start();
        }

        public void Pause()
        {
            CurrentState.Pause();
        }

        public void PauseConsumer()
        {
            Message("pausing");
            consumer.Pause();
        }

        public void PauseProducer()
        {
            Message("pausing");
            producer.Pause();
        }

        public void Resume()
        {
            CurrentState.Resume();
        }

        public void ResumeConsumer()
        {
            Message("resuming");
            consumer.Resume();
        }

        public void ResumeProducer()
        {
            Message("resuming");
            producer.Resume();
        }

        public void Stop()
        {
            CurrentState.Stop();
        }

        public void Wait()
        {
            try
            {
                CurrentState.Wait();
            }
            catch (OperationCanceledException oce)
            {
                Message(oce.ToString());
            }
        }

        public void Dispose()
        {
            CurrentState.Dispose();
        }

        private void Message(string s)
        {
            //ConsolePrint.print("Workflow: " + s);
        }

        private abstract class State
        {
            protected readonly MultithreadWorkflow<T> wf;

            protected State(MultithreadWorkflow<T> wf) { this.wf = wf; }

            protected void Message(string s)
            {
                wf.Message(GetType().Name + " : " + s);
            }

            public virtual void Start() { }
            public virtual void Stop() { }
            public virtual void Pause() { }
            public virtual void Resume() { }
            public virtual void Wait() { }
            public virtual void Finish() { }
            public virtual void Dispose() { }
        }

        private class PendingState : State
        {
            public PendingState(MultithreadWorkflow<T> wf) : base(wf) { }
            public override void Start()
            {
                Message("Starting.");
                wf.producer.Start();
                wf.consumer.Start();
                wf.CurrentState = wf.workingState;
            }
            public override void Dispose()
            {
                wf.producer.Dispose();
                wf.consumer.Dispose();
                wf.CurrentState = wf.disposedState;
            }
        }
        private class WorkingState : State
        {
            public WorkingState(MultithreadWorkflow<T> wf) : base(wf) { }

            public override void Finish()
            {
                wf.CurrentState = wf.finishedState;
            }

            public override void Stop()
            {
                Message("Stopping...");
                wf.cts.Cancel();
                wf.CurrentState = wf.stoppedState;
            }

            public override void Pause()
            {
                Message("pausing");
                wf.producer.Pause();
                wf.consumer.Pause();
                wf.currentState = wf.pausedState;
            }

            public override void Wait()
            {
                Message("Waiting...");
                wf.producer.Wait();
                wf.consumer.Wait();
                Message("Waiting finished.");
            }

            public override void Dispose()
            {
                wf.cts.Cancel();
                Wait();
                wf.producer.Dispose();
                wf.consumer.Dispose();
                wf.CurrentState = wf.disposedState;
            }
        }
        private class PausedState : State
        {
            public PausedState(MultithreadWorkflow<T> wf) : base(wf) { }

            public override void Finish()
            {
                wf.CurrentState = wf.finishedState;
            }

            public override void Resume()
            {
                Message("Resume");
                wf.producer.Resume();
                wf.consumer.Resume();
                wf.CurrentState = wf.workingState;
            }

            public override void Wait()
            {
                Message("Waiting...");
                wf.producer.Wait();
                wf.consumer.Wait();
                Message("Waiting finished.");
            }

            public override void Stop()
            {
                Message("Stopping...");
                wf.cts.Cancel();
                wf.CurrentState = wf.stoppedState;
            }

            public override void Dispose()
            {
                wf.cts.Cancel();
                Wait();
                wf.producer.Dispose();
                wf.consumer.Dispose();
                wf.CurrentState = wf.disposedState;
            }
        }
        private class StoppedState : State
        {
            public StoppedState(MultithreadWorkflow<T> wf) : base(wf) { }
            public override void Dispose()
            {
                Message("Waiting...");
                try
                {
                    wf.producer.Wait();
                    wf.consumer.Wait();
                }
                catch (OperationCanceledException oce)
                {
                    Message(oce.ToString());
                }
                wf.producer.Dispose();
                wf.consumer.Dispose();
                wf.CurrentState = wf.disposedState;
            }
        }
        private class FinishedState : State
        {
            public FinishedState(MultithreadWorkflow<T> wf) : base(wf) { }
            public override void Dispose()
            {
                Message("Waiting...");
                try
                {
                    wf.producer.Wait();
                    wf.consumer.Wait();
                }
                catch (OperationCanceledException oce)
                {
                    Message(oce.ToString());
                }
                wf.producer.Dispose();
                wf.consumer.Dispose();
                wf.CurrentState = wf.disposedState;
            }
        }
        private class DisposedState : State
        {
            public DisposedState(MultithreadWorkflow<T> wf) : base(wf) { }
            public override void Start() { throw new ObjectDisposedException("MultithreadWorflow", "Workflow has been disposed."); }
            public override void Stop() { throw new ObjectDisposedException("MultithreadWorflow", "Workflow has been disposed."); }
            public override void Pause() { throw new ObjectDisposedException("MultithreadWorflow", "Workflow has been disposed."); }
            public override void Resume() { throw new ObjectDisposedException("MultithreadWorflow", "Workflow has been disposed."); }
            public override void Wait() { throw new ObjectDisposedException("MultithreadWorflow", "Workflow has been disposed."); }
            public override void Dispose() { throw new ObjectDisposedException("MultithreadWorflow", "Workflow has been disposed."); }
        }
    }
}
