using System.Threading;
using System.Threading.Tasks;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.Parallel
{
    [RunableClass]
    public class CountDownEventTest
    {
        [Run(0)]
        protected void Test()
        {
            CountdownEvent cde = new CountdownEvent(4);

            var t = Task.Factory.StartNew(() =>
            {
                ConsolePrint.print("[task] {{{0}}} waiting for event...", Task.CurrentId);
                cde.Wait();
                ConsolePrint.print("[task] {{{0}}} finished waiting", Task.CurrentId);
            });

            Thread.Sleep(2000);
            cde.Signal();
            ConsolePrint.print("signalled");
            cde.Signal();
            ConsolePrint.print("signalled");
            Thread.Sleep(1300);
            cde.Signal();
            ConsolePrint.print("signalled");
            Thread.Sleep(300);
            cde.Signal();
            ConsolePrint.print("signalled");
            
            Task.WaitAll(t);
            ConsolePrint.print("[main] finished");
        }
    }
}