using System;
using System.Threading;
using System.Threading.Tasks;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.Parallel
{
    [RunableClass]
    public class TestTasks
    {

        [Run(0)]
        protected void RunTask()
        {
            ConsolePrint.print("before running task");
            Task<int> countFoldersTask = new Task<int>(() =>
                                                           {
                                                               int cnt = 0;

                                                               for (int i = 0; i < 100000; i++)
                                                               {
                                                                   cnt++;
                                                                   Thread.Sleep(0);
                                                               }

                                                               return cnt;
                                                           });
            countFoldersTask.ContinueWith(_ => Finished());

            ConsolePrint.print("started task");

            countFoldersTask.Start();

            ConsolePrint.print("waiting for task finish");
        }


        protected void Finished()
        {
            ConsolePrint.print("finished");
        }


        [Run(0)]
        private static void testParallel()
        {
            System.Threading.Tasks.Parallel.For(0, 1000000, i =>
                                                                {
                                                                    string t = Thread.CurrentThread.Name;
                                                                    Console.Out.WriteLine("i = {0} thread={1}", i, t);
                                                                });
        }

        [Run(0)]
        protected void DoAsyncInit()
        {
            var foo = new ObjectWithAsyncInit();

            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;

            var t = new Task((o) =>
                                 {
                                     var cancel = (CancellationToken)o;
                                     bool isInit;

                                     do
                                     {
                                         isInit = foo.IsInit;
                                         ConsolePrint.print("[{0}] isinit:{1}", DateTime.Now.ToString("HH:mm:ss.ffffff"),
                                                            isInit);
                                         Thread.Sleep(500);
                                     } while (!cancel.IsCancellationRequested && !isInit);
                                 }, ct);
            t.Start();

            Thread.Sleep(5000);

            ConsolePrint.print("cancelling");
            cts.Cancel();
        }

        private class ObjectWithAsyncInit
        {
            private volatile bool init;

            public ObjectWithAsyncInit()
            {
                var t = new Task(Init);
                t.Start();
            }

            public bool IsInit { get { return init; } }

            private void Init()
            {
                Thread.Sleep(700);
                init = true;
            }
        }
    }
}