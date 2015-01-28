using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Experimental.IO;
using log4net;

namespace PDNUtils.Worker
{
    public class LongDirectoryWalker : DirectoryWalker, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LongDirectoryWalker(IEnumerable<string> paths, Action<string> action, bool parallel, IEnumerable<string> ignoredExtensions)
            : base(parallel, ignoredExtensions)
        {
            log.DebugFormat("ctor. {0}", paths);
            if (paths == null) throw new ArgumentNullException("paths");
            if (action == null) throw new ArgumentNullException("action");
            var p = paths.ToList();
            if (log.IsInfoEnabled) log.InfoFormat("Paths: '{0}'", string.Join(";", p));
            this.paths = p;
            this.action = action;
        }

        public void LongWalk()
        {
            if (state == WalkerState.Running || state == WalkerState.Stopping)
            {
                throw new InvalidOperationException(string.Format("walk is {0}", state));
            }

            state = WalkerState.Running;

            Walk(action);

            switch (state)
            {
                case WalkerState.Stopping:
                    state = WalkerState.Stopped;
                    break;
                case WalkerState.Running:
                    state = WalkerState.Finished;
                    break;
            }
        }

        readonly BlockingCollection<string> queue = new BlockingCollection<string>(2048);

        private IEnumerable<string> paths;

        private readonly Action<string> action;

        private static int maxConcurrentProducerTasks = 100; // indeed tasks, not threads

        private static int maxConcurrentConsumerTasks = 100;

        CountdownEvent producerCountdown = new CountdownEvent(1);

        CountdownEvent consumerCountdown = new CountdownEvent(1);

        private SemaphoreSlim producerSignal = new SemaphoreSlim(maxConcurrentProducerTasks, maxConcurrentProducerTasks);

        private SemaphoreSlim consumerSignal = new SemaphoreSlim(maxConcurrentConsumerTasks, maxConcurrentConsumerTasks);

        ManualResetEvent consumerFinish = new ManualResetEvent(false);

        // TODO implement pause with behavior pattern
        ManualResetEvent pause = new ManualResetEvent(true);

        private void Walk(Action<string> action)
        {
            ConsumeItems(action);

            var sw = Stopwatch.StartNew();
            ProduceItems(new CancellationToken());
            sw.Stop();

            consumerFinish.WaitOne();
            log.InfoFormat("Finished producing. Time elapsed: {0}ms", sw.ElapsedMilliseconds);
        }

        private void ConsumeItems(Action<string> action)
        {
            log.Debug("Starting consumer task.");
            ulong c = 0;

            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        log.Info("Starting consuming from queue.");

                        foreach (var item in queue.GetConsumingEnumerable())
                        {
                            c++;
                            log.DebugFormat("Processing #{0} item.", c);
                            var buf = item;
                            ConsumeParallelOrOnCurrent(() => action(buf));
                        }
                        consumerCountdown.Signal();
                        consumerCountdown.Wait();
                    }
                    finally
                    {
                        consumerFinish.Set();
                    }

                    log.InfoFormat("Finished consuming. Consumed {0} items.", c);
                });
        }

        public void ProduceItems(CancellationToken cancel)
        {
            InnerRecursiveWalk(paths, cancel);
            producerCountdown.Signal();
            producerCountdown.Wait();
            queue.CompleteAdding();
        }

        private void InnerRecursiveWalk(IEnumerable<string> paths, CancellationToken cancel)
        {
            foreach (string tmpPath in paths)
            {
                var path = tmpPath;
                log.DebugFormat("Walking in '{0}'.", tmpPath);

                ProduceParallelOrOnCurrent(() =>
                {
                    //Thread.Sleep(100);
                    if (!cancel.IsCancellationRequested)
                    {
                        bool exists = false;

                        try
                        {
                            exists = LongPathDirectory.Exists(path);
                        }
                        catch (UnauthorizedAccessException unae)
                        {
                            log.Error(string.Format("folder [{0}]", path), unae);
                        }

                        if (exists)
                        {
                            try
                            {
                                var pathsBuf = LongPathDirectory.EnumerateFiles(path).ToList();

                                foreach (string s in pathsBuf)
                                {
                                    Add(s);
                                }
                            }
                            catch (UnauthorizedAccessException unae)
                            {
                                log.Error(string.Format("folder [{0}]", path), unae);
                            }

                            try
                            {
                                IEnumerable<string> dirs = LongPathDirectory.EnumerateDirectories(path).ToList();
                                InnerRecursiveWalk(dirs, cancel);
                            }
                            catch (UnauthorizedAccessException unae)
                            {
                                log.Error(string.Format("folder [{0}]", path), unae);
                            }
                        }
                        else
                        {
                            bool b = false;
                            try
                            {
                                b = LongPathFile.Exists(path);
                            }
                            catch (UnauthorizedAccessException unae)
                            {
                                log.Error(string.Format("folder [{0}]", path), unae);
                            }

                            if (b)
                            {
                                Add(path);
                            }
                        }
                    }
                });
            }
        }

        private void ProduceParallelOrOnCurrent(Action action)
        {
            DoParallelOrOnCurrent(action, producerSignal, true);
        }

        private void ConsumeParallelOrOnCurrent(Action action)
        {
            DoParallelOrOnCurrent(action, consumerSignal, false);
        }

        private void DoParallelOrOnCurrent(Action action, SemaphoreSlim signal, bool produce)
        {
            if (IsCancelled())
            {
                return;
            }

            bool parallel = signal.Wait(0);
            //signal.Wait();

            if (parallel)
            {
                if (produce)
                {
                    producerCountdown.AddCount();
                }
                else
                {
                    consumerCountdown.AddCount();
                }

                if (log.IsDebugEnabled) { log.DebugFormat("added {0} task", produce ? "producer" : "consumer"); }

                var t = Task.Factory.StartNew(
                    () =>
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                        finally
                        {
                            signal.Release();

                            if (produce)
                            {
                                producerCountdown.Signal();
                            }
                            else
                            {
                                consumerCountdown.Signal();
                            }

                            log.Debug("finished task");
                        }
                    });
            }
            else
            {
                log.Debug("executing on caller thread");
                action();
            }
        }

        private void Add(string file)
        {
            queue.Add(file);
            
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Added '{0}' to queue. Total items: {1}.", file, queue.Count);
            }
        }

        #region IDisposable
        private bool disposed;

        public void Dispose()
        {
            try
            {
                DisposeInternal();
                GC.SuppressFinalize(this);
                log.Debug("Disposed.");
            }
            catch (Exception e)
            {
                try { log.Error("can't dispose", e); }
                catch { }
            }
        }

        void DisposeInternal()
        {
            if (disposed)
            {
                return;
            }

            if (queue != null) queue.Dispose();
            if (producerCountdown != null) producerCountdown.Dispose();
            if (consumerCountdown != null) consumerCountdown.Dispose();
            if (producerSignal != null) producerSignal.Dispose();
            if (consumerSignal != null) consumerSignal.Dispose();
            if (consumerFinish != null) consumerFinish.Dispose();
            if (pause != null) pause.Dispose();

            disposed = true;
        }
        #endregion

    }
}