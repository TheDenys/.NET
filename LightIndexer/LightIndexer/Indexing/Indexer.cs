using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightIndexer.Config;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using PDNUtils.IO;
using PDNUtils.Worker;
using Utils = PDNUtils.Help.Utils;

namespace LightIndexer.Indexing
{
    public class Indexer : IDisposable
    {
        // turns on/off parallel indexing
        private const bool parallel = true;

        private readonly CancellationTokenSource cancellationSource;

        //logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ManualResetEvent waitHandle = new ManualResetEvent(true);

        public event Action<ProgressInfo> RefreshEvent;

        private readonly IEnumerable<FileSystemInfo> dirs;

        private readonly Thread workerThread;

        private readonly object sync = new object();

        private readonly object sync_finish = new object();

        private LongDirectoryWalker walker;

        private long _total = -1, _completed;

        private Exception LastError;

        public Indexer(IEnumerable<string> paths)
        {
            cancellationSource = new CancellationTokenSource();

            // gets only existing directories
            dirs = paths.
                Where(p => (Directory.Exists(p) || File.Exists(p))).
                Select<string, FileSystemInfo>(p =>
                           {
                               FileAttributes attr = File.GetAttributes(p);

                               if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                               {
                                   return new DirectoryInfo(p);
                               }
                               else
                               {
                                   return new FileInfo(p);
                               }
                           });

            workerThread = new Thread(longProcess);

            this.pendingState = new PendingState(this);
            this.indexingState = new IndexingState(this);
            this.pausedState = new PausedState(this);
            this.stoppingState = new StoppingState(this);
            this.stoppedState = new StoppedState(this);
            this.finishedState = new FinishedStated(this);
            this.failedState = new FailedSate(this);
            this.currentState = pendingState;
        }

        public void RaiseEvent()
        {
            //waitHandle.WaitOne();

            lock (sync)
            {
                if (RefreshEvent != null)
                {
                    ProgressInfo info = new ProgressInfo(_completed, _total);
                    RefreshEvent(info);
                }
            }
        }

        public void Pause() { currentState.Pause(); }
        public void Resume() { currentState.Resume(); }
        public void Start() { currentState.Start(); }
        public void Stop() { currentState.Stop(); }
        public bool IsRunning { get { return currentState.IsRunning; } }
        public IndexerResult Result { get { return currentState.Result; } }
        // wait for finishing all threads
        // use this method to be asured that scan process have been finished
        public void WaitFor() { currentState.WaitFor(); }

        private void longProcess()
        {
            log.Info("indexing started");

            try
            {
                // count amount of file in directories and write result to _total
                Task<long> countFilesTask = new Task<long>(() =>
                {
                    log.Info("started counting files");
                    return Utils.GetFilesCount(dirs.OfType<DirectoryInfo>(), null);
                });

                // update _total
                Task t = countFilesTask.ContinueWith(countTask =>
                                                         {
                                                             lock (sync) _total = countTask.Result;
                                                             log.InfoFormat("finished counting files: {0}", _total);
                                                         }
                    );

                // raise event when count has finished
                t.ContinueWith(_ => RaiseEvent());

                currentState.PauseWait();
                // start counting task
                countFilesTask.Start();

                currentState.PauseWait();
                RaiseEvent();
                var sw = new Stopwatch();

                // do indexing
                using (IndexManager defaultIndexManager = Configurator.GetDefaultIndexManager())
                {
                    currentState.PauseWait();

                    using (var writer = defaultIndexManager.OpenIndexWriter())
                    {
                        if (writer == null)
                            throw new ApplicationException("Can't open index.");

                        sw.Start();

                        var paths = dirs.Select(d => d.FullName);
                        Action<string> action = fi =>
                        {
                            currentState.PauseWait();
                            HandleFile(fi, writer);
                            Interlocked.Increment(ref _completed);
                            RaiseEvent();
                            currentState.PauseWait();
                        };

                        walker = new LongDirectoryWalker(paths, action, parallel, null);
                        walker.LongWalk();

                        sw.Stop();

                        double totalSeconds = sw.Elapsed.TotalSeconds;
                        log.InfoFormat("{0} indexing finished in {1}s", walker.IsParallel ? "parallel" : "sequential",
                            totalSeconds);

                        currentState.PauseWait();
                        sw.Restart();
                        currentState.Optimize(writer);
                        sw.Stop();

                        double optimizeSeconds = sw.Elapsed.TotalSeconds;
                        totalSeconds += optimizeSeconds;
                        log.InfoFormat("index optimized in {0}s total elapsed:{1}s", optimizeSeconds, totalSeconds);
                    }
                }


                if (log.IsDebugEnabled)
                {
                    using (IndexManager defaultIndexManager = Configurator.GetDefaultIndexManager())
                    using (var r = defaultIndexManager.GetOpenIndexReader())
                    {
                        log.DebugFormat("index contains {0} documents", r.NumDocs());
                    }
                }

                RaiseEvent();
                currentState.SetState(finishedState);
            }
            catch (Exception ex)
            {
                LastError = ex;
                currentState.SetState(failedState);
                log.Error("Exception in indexing operation.", ex);

                if (ex is ThreadAbortException)
                {
                    log.Info("aborted");
                }
            }
            finally
            {
                RaiseEvent();
                try
                {
                    lock (sync_finish)
                    {
                        Monitor.Pulse(sync_finish);
                    }
                }
                catch (Exception e)
                {
                    log.Error("exception in longProcess", e);
                }
            }
        }

        private static void HandleFile(string fi, IndexWriter writer)
        {
            try
            {
                // TODO: make it nice, i.e. create 2 behaviors JustFiles, FilesAndArchives
                // set them according to config or user preference
                if (fi.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
                {
                    HandleZipFile(fi, writer);
                }
                else
                {
                    HandlePlainFile(fi, writer);
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("failed to index {0}", fi), e);
            }
        }

        private static void HandlePlainFile(string fi, IndexWriter writer)
        {
            Document doc;
            using (var iDoc = DocumentBuilder.LongGetDocument(fi))
            {
                doc = iDoc.Doc;

                if (doc != null)
                {
                    writer.AddDocument(doc);
                }
            }
        }

        private static void HandleZipFile(string fi, IndexWriter writer)
        {
            var filesInZip = ZipHelper.GetFilesFromZip(fi, Constants.MAX_FILE_SIZE);
            Document doc;

            foreach (Tuple<string, Stream> tuple in filesInZip)
            {
                if (tuple == null)
                {
                    continue;
                }

                using (var iDoc = DocumentBuilder.GetDocumentFromStream(tuple.Item1, tuple.Item2))
                {
                    doc = iDoc.Doc;

                    if (doc != null)
                    {
                        writer.AddDocument(doc);
                    }
                }
            }
        }

        private void WaitForIndexingFinishedInternal()
        {
            try
            {
                lock (sync_finish)
                    Monitor.Wait(sync_finish);
            }
            catch (Exception e)
            {
                log.Error(e, e);
            }
        }

        #region IDisposable
        private bool disposed;

        public void Dispose()
        {
            Dispose(true); //true: safe to free managed resources
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool isDisposing)
        {
            if (disposed)
            {
                return;
            }

            if (isDisposing)
            {
                // release managed
                waitHandle.Dispose();
                cancellationSource.Dispose();
            }

            disposed = true;
        }

        ~Indexer()
        {
            Dispose(false);
        }

        #endregion IDisposable

        private IndexerStateBase currentState;
        private readonly IndexerStateBase pendingState;
        private readonly IndexerStateBase indexingState;
        private readonly IndexerStateBase pausedState;
        private readonly IndexerStateBase stoppingState;
        private readonly IndexerStateBase stoppedState;
        private readonly IndexerStateBase failedState;
        private readonly IndexerStateBase finishedState;

        private void SwitchState(IndexerStateBase target)
        {
            currentState.SetState(target);
        }

        private abstract class IndexerStateBase
        {
            protected Indexer ctx;
            protected IndexerStateBase(Indexer ctx) { this.ctx = ctx; }
            public virtual bool IsRunning { get { return false; } }

            public void SetState(IndexerStateBase target)
            {
                if (CanSwitch(target))
                {
                    target.Activate();
                }
                else
                {
                    throw new InvalidOperationException("can't switch to state " + target);
                }
            }

            public virtual bool CanSwitch(IndexerStateBase target) { return true; }

            public virtual void Activate() { ctx.currentState = this; }
            public virtual void Start() { }
            public virtual void Stop() { }
            public virtual void Pause() { }
            public virtual void PauseWait() { }
            public virtual void RaiseEvent() { ctx.RaiseEvent(); }
            public virtual void Resume() { }
            public virtual void WaitFor() { }
            public virtual void Optimize(IndexWriter writer) { }
            public abstract IndexerResult Result { get; }
        }

        private class PendingState : IndexerStateBase
        {
            public PendingState(Indexer ctx) : base(ctx) { }
            public override void Start()
            {
                ctx.workerThread.Start();
                SetState(ctx.indexingState);
            }

            public override IndexerResult Result { get { throw new InvalidOperationException("No result yet. Try start indexer."); } }
        }

        private class IndexingState : IndexerStateBase
        {
            public IndexingState(Indexer ctx) : base(ctx) { }
            public override bool IsRunning { get { return true; } }
            public override void Stop()
            {
                SetState(ctx.stoppingState);
            }

            public override void Pause()
            {
                ctx.waitHandle.Reset();
                SetState(ctx.pausedState);
            }

            public override void WaitFor() { ctx.WaitForIndexingFinishedInternal(); }

            public override void Optimize(IndexWriter writer)
            {
                log.Info("started index optimizing");
                var sw2 = Stopwatch.StartNew();
                writer.Optimize();
                sw2.Stop();
                log.InfoFormat("index optimized in {0}s", sw2.Elapsed.TotalSeconds);
            }

            public override IndexerResult Result { get { throw new InvalidOperationException("No result yet. Try to wait for finishing."); } }
        }

        private class PausedState : IndexerStateBase
        {
            public PausedState(Indexer ctx) : base(ctx) { }
            public override void WaitFor() { ctx.WaitForIndexingFinishedInternal(); }
            public override void PauseWait() { ctx.waitHandle.WaitOne(); }
            public override void Resume()
            {
                ctx.waitHandle.Set();
                SetState(ctx.indexingState);
            }
            public override void Stop()
            {
                ctx.waitHandle.Set();
                SetState(ctx.stoppingState);
            }
            public override IndexerResult Result { get { throw new InvalidOperationException("Paused."); } }
        }

        private class StoppingState : IndexerStateBase
        {
            public StoppingState(Indexer ctx) : base(ctx) { }
            public override bool CanSwitch(IndexerStateBase target)
            {
                return target is StoppedState || target is FailedSate;
            }

            public override void Activate()
            {
                base.Activate();
                ctx.cancellationSource.Cancel();
                ctx.waitHandle.Set();

                try
                {
                    lock (ctx.sync)
                    {
                        if (ctx.workerThread != null && ctx.workerThread.IsAlive)
                        {
                            ctx.walker.Stop();
                        }
                    }

                    SetState(ctx.stoppedState);
                }
                catch (Exception)
                {
                    SetState(ctx.failedState);
                }
            }

            public override IndexerResult Result { get { throw new InvalidOperationException("Stopping."); } }
        }

        private class StoppedState : IndexerStateBase
        {
            public StoppedState(Indexer ctx) : base(ctx) { }
            public override IndexerResult Result { get { return new IndexerResult(false, "Indexer was stopped.", ctx.LastError); } }
        }

        private class FailedSate : IndexerStateBase
        {
            public FailedSate(Indexer ctx) : base(ctx) { }
            public override IndexerResult Result { get { return new IndexerResult(false, "Indexer has failed.", ctx.LastError); } }
        }

        private class FinishedStated : IndexerStateBase
        {
            public FinishedStated(Indexer ctx) : base(ctx) { }
            public override bool CanSwitch(IndexerStateBase target) { return false; }
            public override IndexerResult Result { get { return new IndexerResult(true); } }
        }
    }
}