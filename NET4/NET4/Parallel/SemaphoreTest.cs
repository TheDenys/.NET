using System;
using System.Threading;
using System.Threading.Tasks;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.Parallel
{
    [RunableClass]
    public class SemaphoreTest
    {
        [Run(0)]
        protected void Test()
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(0, 2);

            Action action = () =>
                {
                    ConsolePrint.print("[task] {{{0}}} waiting for sempahore...", Task.CurrentId);
                    semaphore.Wait();
                    ConsolePrint.print("[task] {{{0}}} finished waiting", Task.CurrentId);
                };

            var t = Task.Factory.StartNew(action);
            var t2 = Task.Factory.StartNew(action);
            var t3 = Task.Factory.StartNew(action);
            var t4 = Task.Factory.StartNew(action);

            ConsolePrint.print("[main] delay before releasing semaphore (2)");
            Thread.Sleep(5000);
            semaphore.Release(2);
            ConsolePrint.print("[main] semaphore released (2)");
            ConsolePrint.print("[main] delay before releasing semaphore (2 more)");
            Thread.Sleep(2000);
            semaphore.Release(2);
            ConsolePrint.print("[main] semaphore released (2 more)");
            ConsolePrint.print("[main] waiting for task finish...");
            Task.WaitAll(t, t2, t3, t4);
            ConsolePrint.print("[main] task completed");
        }
    }
}