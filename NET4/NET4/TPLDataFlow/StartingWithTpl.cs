using System.Threading;
using System.Threading.Tasks.Dataflow;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TPLDataFlow
{
    [RunableClass]
    public class StartingWithTpl
    {
        [Run(0)]
        protected void Test()
        {
            var actionBlock = new ActionBlock<int>(delegate(int i)
                {
                    ConsolePrint.print("computing i:{0} thread:", i, Thread.CurrentThread.ManagedThreadId);
                    Thread.Sleep(1200);
                    ConsolePrint.print("finished computing {0}", i);
                });

            actionBlock.Post(1);
            actionBlock.Post(2);
            actionBlock.Post(3);
        }
    }
}