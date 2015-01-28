using System;
using System.Globalization;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;
using System.Threading;
using log4net;

namespace NET4.TestClasses
{
    [RunableClass]
    public class TimerTasks
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Run(0)]
        protected void ScheduleAt()
        {
            var timerDisposed = new ManualResetEvent(false);
            var done = new ManualResetEventSlim(false);
            //TODO
            var now = DateTime.Now;
            now = DateTime.Parse("2013-02-16T10:58:00", CultureInfo.InvariantCulture);
            log.DebugFormat("Now:{0}", now);
            var runAt = new TimeSpan(now.Hour, now.Minute, now.Second + 5);
            runAt = TimeSpan.Parse("10:55", CultureInfo.InvariantCulture);
            log.DebugFormat("Scheduled run at: {0}", runAt);
            var dueTime = runAt - now.TimeOfDay;
            log.DebugFormat("DueTime is {0}", dueTime);
            if (dueTime < TimeSpan.Zero)
            {
                dueTime = dueTime.Add(TimeSpan.FromDays(1));
                log.DebugFormat("DueTime recalculated to {0}", dueTime);
            }

            Timer timer = new Timer(_ =>
                {
                    log.DebugFormat("timer tick at: {0}", DateTime.Now);
                    done.Set();
                });

            var period = TimeSpan.FromSeconds(5);
            timer.Change(dueTime, period);
            log.DebugFormat("Change timer ({0},{1})", dueTime, period);
            log.DebugFormat("Waiting {0}", dueTime);
            done.Wait();

            timer.Dispose();
            //or
            //if (!timer.Dispose(timerDisposed))
            //{
            //    timerDisposed.WaitOne();
            //}
            log.Debug("Timer disposed.");
        }

        [Run(0)]
        protected void UseTimer()
        {
            Timer t = new Timer((state) =>
                                    {
                                        ConsolePrint.print("tick...");
                                        var tmr = (Timer)state;
                                        tmr.Change(Timeout.Infinite, Timeout.Infinite);
                                        Thread.Sleep(5000);
                                        tmr.Change(0, 1000);
                                    });
            t.Change(0, 500);
        }


        Thread consumer1, consumer2;

        ManualResetEventSlim consumerWait = new ManualResetEventSlim(true);

        private Timer t;

        [Run(0)]
        public void WaitHandleSyncStart()
        {
            t = new Timer((state) =>
            {
                var tmr = (Timer)state;
                tmr.Change(Timeout.Infinite, Timeout.Infinite);
                ConsolePrint.print("working...");
                Thread.Sleep(5000);
                bool canConsumerContinue = CanConsumerContinue();
                ConsolePrint.print("canContinue: " + canConsumerContinue);

                if (canConsumerContinue)
                {
                    consumerWait.Set();
                    tmr.Change(Timeout.Infinite, Timeout.Infinite);
                }
                else
                {
                    tmr.Change(0, Timeout.Infinite);
                }
            });
            t.Change(0, Timeout.Infinite);


            consumer1 = new Thread(ConsumerMethod);
            consumer2 = new Thread(ConsumerMethod);

            consumer1.Start();
            consumer2.Start();

            var thread = new Thread(() =>
                                        {
                                            ConsolePrint.print("started managing thread");
                                            Thread.Sleep(5000);
                                            ConsolePrint.print("aborting...");
                                            t.Dispose();
                                            consumer1.Abort();
                                            consumer2.Abort();
                                            ConsolePrint.print("abort called...");
                                        });
            //thread.Start();
        }

        public void WaitForFinish()
        {
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
            t.Change(0, Timeout.Infinite);
            consumerWait.Reset();
        }

    }

}