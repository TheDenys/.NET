using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class TestRx
    {
        /// <summary>
        /// Test RX
        /// </summary>
        [Run(0)]
        public static void Rx()
        {
            var run = Observable.Start(() =>
                                           {
                                               ConsolePrint.print("Calcs...");
                                               Thread.Sleep(1500);
                                               ConsolePrint.print("finish");
                                           });
            ConsolePrint.print("something");
            run.First();

            var xs = Observable.Range(5, 10);// Interval(TimeSpan.FromMilliseconds(100));
            {
                using(var o = xs.SubscribeOn(SynchronizationContext.Current).Subscribe(
                    i => ConsolePrint.print(i),
                    () => ConsolePrint.print("finished")
                    ))
                {
                    Thread.Sleep(1500);
                }
            }
        }

        [Run(0)]
        protected void TryLongAsync()
        {
            IObservable<int> ob = Observable.Create<int>(o =>
                                                             {
                                                                 Scheduler.NewThread.Schedule(() =>
                                                                                                  {
                                                                                                      ConsolePrint.print("start long, t=" + Thread.CurrentThread.ManagedThreadId);
                                                                                                      Thread.Sleep(1500);
                                                                                                      ConsolePrint.print("finish long, t=" + Thread.CurrentThread.ManagedThreadId);
                                                                                                      o.OnCompleted();
                                                                                                  });
                                                                 return ()=> { };
                                                             });
            ob.SubscribeOn(SynchronizationContext.Current).Subscribe((int i) => { }, () => ConsolePrint.print("start eeeeee, t="+Thread.CurrentThread.ManagedThreadId));
            ConsolePrint.print("gotcha");
            Thread.Sleep(2000);
            ConsolePrint.print("gotcha 3");
        }

        protected void DoNothing()
        {
            ConsolePrint.print("start long, t=" + Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(3000);
            ConsolePrint.print("finish long, t=" + Thread.CurrentThread.ManagedThreadId);
        }

        [Run(0)]
        protected void ToAsync()
        {
            Observable.ToAsync(DoNothing)().ObserveOn(SynchronizationContext.Current).Subscribe(
                (result) => { ConsolePrint.print("result, t=" + Thread.CurrentThread.ManagedThreadId); },//onnext
                //(ex) => { },//onerror
                () => { ConsolePrint.print("completed long, t=" + Thread.CurrentThread.ManagedThreadId); }//oncompleted
                );
        }
    }
}