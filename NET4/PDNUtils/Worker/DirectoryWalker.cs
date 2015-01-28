using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using log4net;
using PDNUtils.Help;

namespace PDNUtils.Worker
{
    /// <summary>
    /// Class for executing actions against files in directory
    /// </summary>
    public class DirectoryWalker
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly bool _parallel;

        protected WalkerState state = WalkerState.Clean;

        protected volatile int _currentProcessed;

        protected IEnumerable<string> ignoredExtensions;


        protected readonly object _ignoredExtensionsCacheLocker = new object();
        protected ISet<string> ignoredExtensionsCache = new HashSet<string>();

        /// <summary>
        /// Default constructor. Uses sequential execution method and unlimited path depth.
        /// </summary>
        public DirectoryWalker()
        {
        }

        /// <summary>
        /// Uses unlimited path depth.
        /// </summary>
        /// <param name="parallel">sets the execution type. if true then parallel</param>
        public DirectoryWalker(bool parallel)
            : this()
        {
            _parallel = parallel;
        }

        /// <summary>
        /// Uses given execution type and depth and ignores files with extnesions from <paramref name="ignoredExtensions"/>
        /// </summary>
        /// <param name="parallel">sets the execution type. if true then parallel</param>
        /// <param name="ignoredExtensions">collection of extension to be ignored</param>
        public DirectoryWalker(bool parallel, IEnumerable<string> ignoredExtensions)
            : this(parallel)
        {
            this.ignoredExtensions = ignoredExtensions;
        }

        /// <summary>
        /// Gets current wlaker state
        /// </summary>
        public WalkerState State { get { return state; } }

        /// <summary>
        /// Executes <paramref name="action"/> against files in <paramref name="directories"/>
        /// </summary>
        /// <param name="directories">collection of <see cref="DirectoryInfo"/></param>
        /// <param name="action">action to be executed against file</param>
        public void Walk(IEnumerable<DirectoryInfo> directories, Action<FileInfo> action)
        {
            if (state == WalkerState.Running || state == WalkerState.Stopping)
            {
                throw new InvalidOperationException(string.Format("walk is {0}", state));
            }

            state = WalkerState.Running;

            directories.CancellableForEach(di => WalkSingle(di, action), _parallel, IsCancelled);

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

        /// <summary>
        /// Executes <paramref name="action"/> against files in <paramref name="directory"/>
        /// To walk over few directories use overloaded method
        /// </summary>
        /// <param name="directory">instance of <see cref="DirectoryInfo"/></param>
        /// <param name="action">action to be executed against file</param>
        public void Walk(DirectoryInfo directory, Action<FileInfo> action)
        {
            if (state == WalkerState.Running || state == WalkerState.Stopping)
            {
                throw new InvalidOperationException(string.Format("walk is {0}", state));
            }

            state = WalkerState.Running;
            WalkSingle(directory, action);

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

        /// <summary>
        /// Walks a single directory
        /// </summary>
        /// <param name="di"></param>
        /// <param name="action"></param>
        protected void WalkSingle(DirectoryInfo di, Action<FileInfo> action)
        {
            if (log.IsInfoEnabled)
            {
                log.InfoFormat("started Walk in folder [{0}], depth:{1}, parallel:{2}", di.FullName, "all", _parallel);
            }

            InnerRecursiveWalk(di, action);

            if (log.IsInfoEnabled)
            {
                log.InfoFormat("finished Walk in folder [{0}], depth:{1}, parallel:{2}", di.FullName, "all", _parallel);
            }
        }

        /// <summary>
        /// Calls the execution stop. Currently executing actions will be finished though.
        /// </summary>
        public void Stop()
        {
            if (state != WalkerState.Aborted && state != WalkerState.Finished && state != WalkerState.Stopped)
            {
                state = WalkerState.Stopping;
            }

            if (log.IsInfoEnabled)
            {
                log.Info("called stop");
            }
        }

        /// <summary>
        /// Amount of processd files by this moment.
        /// </summary>
        public int ItemsProcessed { get { return _currentProcessed; } }

        /// <summary>
        /// Shows if the executing is parallel or sequentilal
        /// </summary>
        public bool IsParallel { get { return _parallel; } }

        protected void InnerRecursiveWalk(DirectoryInfo directory, Action<FileInfo> action)
        {
            try
            {
                // depth == null means no limit
                if (!IsCancelled())
                {
                    directory.GetFiles().CancellableForEach(fi => DoFileAction(fi, action), _parallel, IsCancelled);

                    if (!IsCancelled())
                    {
                        directory.GetDirectories().CancellableForEach(dir => InnerRecursiveWalk(dir, action), _parallel, IsCancelled);
                    }
                }

            }
            catch (UnauthorizedAccessException unae)
            {
                if (log.IsDebugEnabled)
                {
                    log.Error(string.Format("folder [{0}]", directory.FullName), unae);
                }
                else
                {
                    log.Error(unae.Message);
                }
            }
        }

        protected void DoFileAction(FileInfo fi, Action<FileInfo> action)
        {
            Interlocked.Increment(ref _currentProcessed);

            if (Skip(fi))
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("skip {0}", fi.Name);
                }
                return;
            }

            try
            {
                action(fi);
            }
            catch (Exception e)
            {
                log.ErrorFormat("action failed for file [{0}]", fi.FullName);
                log.Warn(e);
            }
        }

        private bool Skip(FileInfo fi)
        {
            if (ignoredExtensions == null || ignoredExtensions.Count() == 0)
            {
                return false;
            }
            var fileExt = fi.Extension.ToLowerInvariant();
            if (ignoredExtensionsCache.Contains(fileExt))
            {
                return true;
            }
            else
            {
                var skip =
                    ignoredExtensions.Where(ext => fi.Extension != null && fi.Extension.ToLowerInvariant().Contains(ext)).Count() > 0;

                if (skip)
                {
                    lock (_ignoredExtensionsCacheLocker)
                    {
                        ignoredExtensionsCache.Add(fileExt);
                    }
                }

                return skip;
            }
        }

        protected bool IsCancelled()
        {
            return state == WalkerState.Stopping;
        }

    }

    public enum WalkerState
    {
        Clean,
        Running,
        Finished,
        Aborted,
        Stopped,
        Stopping
    }

}