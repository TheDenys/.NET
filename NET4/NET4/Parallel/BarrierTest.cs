using System;
using System.Threading;
using System.Threading.Tasks;
using PDNUtils.Help;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Parallel
{
    [RunableClass]
    public class BarrierTest : RunableBase
    {
        [Run(0)]
        protected void Test()
        {
            ConsolePrint.ShowTime = true;
            Action<Barrier> postAction = b => DebugFormat("post action phase {0}", b.CurrentPhaseNumber);
            Barrier barrier = new Barrier(3, postAction);

            Action action1 = () =>
            {
                Debug("action1, doing task...");
                Thread.Sleep(2500);
                Debug("action1 done and waiting...");
                barrier.SignalAndWait();
                Debug("action1 exit.");
            };

            Action action2 = () =>
            {
                Debug("action2, doing task...");
                Thread.Sleep(4500);
                Debug("action2 done and waiting...");
                barrier.SignalAndWait();
                Debug("action2 exit.");
            };

            var t1 = Task.Factory.StartNew(action1);
            var t2 = Task.Factory.StartNew(action1);
            var t3 = Task.Factory.StartNew(action2);

            Task.WaitAll(t1, t2, t3);

            Debug("main finished");
        }
    }
}