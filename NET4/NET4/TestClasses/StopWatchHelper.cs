using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using log4net;

namespace NET4.TestClasses
{
    public class StopWatchHelper
    {

        private readonly string name;

        private Stopwatch globalStopWatch = new Stopwatch();

        private IDictionary<string, Stopwatch> stopWatches = new Dictionary<string, Stopwatch>();

        private readonly object locker = new object();

        public StopWatchHelper(string name)
            : this(name, true)
        {
        }

        public StopWatchHelper(string name, bool startImmediately)
        {
            this.name = name;
            if (startImmediately)
            {
                globalStopWatch.Start();
            }
        }

        public IEnumerable<KeyValuePair<string, long>> Results
        {
            get
            {
                lock (stopWatches)
                {
                    var dict = stopWatches.Select(entry => new KeyValuePair<string, long>(entry.Key, entry.Value.ElapsedMilliseconds));
                    return dict;
                }
            }
        }

        public void BeginSection(string p0)
        {
            AddSection(p0);
        }

        public void EndSection(string name)
        {
            Stopwatch stopWatch = null;
            bool has = false;

            lock (locker)
            {
                has = stopWatches.TryGetValue(name, out stopWatch);
            }

            if (has && stopWatch != null)
            {
                stopWatch.Stop();
            }
        }

        public void BeginGlobal()
        {
            if (!globalStopWatch.IsRunning)
            {
                globalStopWatch.Start();
            }
        }

        public void EndGlobal()
        {
            globalStopWatch.Stop();
        }

        public void PrintResults(ILog log)
        {
            EndGlobal();

            if (!log.IsDebugEnabled)
            {
                return;
            }

            var t = new Thread(() =>
                                   {
                                       lock (locker)
                                       {
                                           //stop all
                                           foreach (var stopwatch in stopWatches.Values)
                                           {
                                               if (stopwatch != null)
                                               {
                                                   stopwatch.Stop();
                                               }
                                           }
                                           var res = from entry in stopWatches
                                                     select
                                                         string.Format("Section '{0}': {1}ms", entry.Key,
                                                                       entry.Value.ElapsedMilliseconds);
                                           var outString = string.Join("\n", res);
                                           outString = string.Format("StopWatch '{0}'\n{1}", name, outString);
                                           //add total
                                           outString += string.Format("\nTotal('{0}'): {1}ms", name, globalStopWatch.ElapsedMilliseconds);
                                           log.Debug(outString);
                                       }
                                   });
            t.Start();
        }

        private void AddSection(string name)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var t = new Thread(() =>
                                   {
                                       lock (locker)
                                       {
                                           if (string.IsNullOrEmpty(name) || stopWatches.ContainsKey(name))
                                           {
                                               name = string.Format("{0}-{1}", Guid.NewGuid().ToString(), name);
                                           }
                                           stopWatches.Add(new KeyValuePair<string, Stopwatch>(name, stopWatch));
                                       }
                                   });
            t.Start();
        }

        private void RemoveSection(string name)
        {
            var t = new Thread(() =>
                                   {
                                       lock (locker)
                                       {
                                           stopWatches.Remove(name);
                                       }
                                   });
            t.Start();
        }

    }
}