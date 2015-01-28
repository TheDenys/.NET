using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;

namespace PDNUtils.Help
{
    /// <summary>
    /// extension methods for iterations
    /// </summary>
    public static class ForEachExtensionMethods
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // seems doesn't have any effect
        private static int _maxDegreeOfParallelism = Environment.ProcessorCount;

        /// <summary>
        /// foreach
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <param name="parallel"></param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action, bool parallel)
        {
            if (parallel)
            {
                var opt = new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism };
                Parallel.ForEach(
                    source,
                    opt, 
                    action);
            }
            else
            {
                foreach (var v in source) { action(v); }
            }
        }

        /// <summary>
        /// foreach with possibility to cancel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <param name="parallel"></param>
        /// <param name="isCancelled"></param>
        /// <returns></returns>
        public static ParallelLoopResult CancellableForEach<T>(this IEnumerable<T> source, Action<T> action, bool parallel, Func<bool> isCancelled)
        {
            if (parallel)
            {
                var opt = new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism };
                return Parallel.ForEach(
                    source,
                    opt,
                    (i, loopState) =>
                    {
                        if (isCancelled != null && isCancelled())
                        {
                            loopState.Stop();
                        }

                        try
                        {
                            action(i);
                        }
                        catch (Exception e)
                        {
                            log.Error(e, e);
                            throw;
                        }
                    }
                );
            }
            else
            {
                var fakeParallelLoopResult = new ParallelLoopResult();

                foreach (var v in source)
                {
                    if (isCancelled != null && isCancelled())
                    {
                        return fakeParallelLoopResult;
                    }

                    action(v);
                }

                return fakeParallelLoopResult;
            }
        }

        /// <summary>
        /// parallel foreach
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void ForEachParallel<T>(this IEnumerable<T> source, Action<T> action)
        {
            ForEach(source, action, true);
        }
    }
}