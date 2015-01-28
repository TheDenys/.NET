using System;
using System.Threading;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class WaitHandleSync
    {

        Thread worker, consumer1, consumer2;

        ManualResetEventSlim workerWait = new ManualResetEventSlim(false);

        ManualResetEventSlim consumerWait = new ManualResetEventSlim(true);

        [Run(0)]
        public void WaitHandleSyncStart()
        {
            worker = new Thread(WorkerMethod);
            consumer1 = new Thread(ConsumerMethod);
            consumer2 = new Thread(ConsumerMethod);

            worker.Start();
            consumer1.Start();
            consumer2.Start();

            var thread = new Thread(() =>
                                        {
                                            ConsolePrint.print("started managing thread");
                                            Thread.Sleep(5000);
                                            ConsolePrint.print("aborting...");
                                            worker.Abort();
                                            consumer1.Abort();
                                            consumer2.Abort();
                                            ConsolePrint.print("abort called...");
                                        });
            //thread.Start();
        }

        public void WaitForFinish()
        {
        }

        private void WorkerMethod()
        {
            while (true)
            {
                workerWait.Wait();
                ConsolePrint.print("working...");
                Thread.Sleep(1500);

                bool canConsumerContinue = CanConsumerContinue();

                ConsolePrint.print("canContinue: " + canConsumerContinue);

                if (canConsumerContinue)
                {
                    consumerWait.Set();
                    workerWait.Reset();
                }
            }
        }

        private void ConsumerMethod()
        {
            while (true)
            {
                WaitForConnect();
                ConsolePrint.print("consuming...");
                //Thread.Sleep(150);
                //Thread.SpinWait(200000000);

                if (!CanConsumerContinue())
                {
                    ReportError();
                }
            }
        }

        private bool CanConsumerContinue()
        {
            bool canConsumerContinue = DateTime.Now.Second <= 30;
            ConsolePrint.print("returning {0} for can Continue", canConsumerContinue);
            return canConsumerContinue;
        }

        private void WaitForConnect()
        {
            ConsolePrint.print("awaiting for connect...");
            consumerWait.Wait();
        }

        private void ReportError()
        {
            ConsolePrint.print("report error");
            workerWait.Set();
            consumerWait.Reset();
        }

    }
}
