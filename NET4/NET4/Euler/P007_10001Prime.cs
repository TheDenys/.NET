using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using PDNUtils.Help;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P007_10001Prime : RunableBase
    {
        [Run(0)]
        protected void SolveIt()
        {
            ConsolePrint.ShowTime = true;

            byte parallelism = (byte)Environment.ProcessorCount;

            CancellationTokenSource cts = new CancellationTokenSource();
            NumMaker nm = new NumMaker(cts.Token);
            var token = cts.Token;

            long p = 1;
            int pos = 0;
            long prime = 0;

            Action<object> action = (__procId) =>
            {
                SetThreadProcessorAffinity((int)__procId);

                while (pos < 10001 && !token.IsCancellationRequested)
                {
                    long i = Interlocked.Increment(ref p);
                    if (Common.IsPrime(i))
                    {
                        int curpos = Interlocked.Increment(ref pos);
                        //DebugFormat("i: {0} prime: {1} pos:{2}", i, IsPrime(i), curpos);
                        if (curpos == 10001)
                        {
                            prime = i;
                            DebugFormat("######## i: {0} prime: {1} pos:{2}", i, Common.IsPrime(i), curpos);
                            break;
                        }
                    }
                    //Thread.Yield();
                }

                //for (int i = 0; i < 1000000000; i++)
                //{
                //    double z = 0;
                //    for (int noop = 0; noop < 100000; noop++)
                //    {
                //        z = (double)i / (double)(i + 1) * (double)(i + 2);
                //    }

                //    if (i % 100 == 0)
                //    {
                //        DebugFormat("i: {0} res: {1}", i, z);
                //    }
                //}
            };

            Thread[] threads = new Thread[parallelism];

            for (int cpu = 0; cpu < parallelism; cpu++)
            {
                Thread t = new Thread(o => action(o));
                t.IsBackground = true;
                t.Name = "worker " + cpu;
                t.Priority = ThreadPriority.Highest;
                t.Start(cpu);
                threads[cpu] = t;
            }

            //Thread.Sleep(15000);

            //cts.Cancel();

            for (int cpu = 0; cpu < parallelism; cpu++)
            {
                threads[cpu].Join();
            }

            Debug("main finished");
        }

        public class NumMaker : IEnumerable<long>
        {
            NumMakerEnum en;

            public NumMaker(CancellationToken cancel)
            {
                en = new NumMakerEnum(cancel);
            }

            public IEnumerator<long> GetEnumerator()
            {
                return en;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private class NumMakerEnum : IEnumerator<long>
            {
                private object sync = new object();
                private CancellationToken cancel;
                long i;

                public NumMakerEnum(CancellationToken cancel)
                {
                    this.cancel = cancel;
                    Current = 1;
                }

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    if (!cancel.IsCancellationRequested)
                    {
                        lock (sync)
                        {
                            Current = Current + 1;
                        }

                        return true;
                    }

                    return false;
                }

                public void Reset()
                {
                    lock (sync)
                        Current = 0;
                }

                public long Current { get; private set; }

                object IEnumerator.Current
                {
                    get
                    {
                        lock (sync)
                            return Current;
                    }
                }
            }
        }

        protected static void SetThreadProcessorAffinity(int cpu)
        {
            if (cpu > Environment.ProcessorCount)
                throw new ArgumentOutOfRangeException("Invalid CPU number.");

            Thread.BeginThreadAffinity();
#pragma warning disable 618
            int osThreadId = AppDomain.GetCurrentThreadId();
#pragma warning enable 618
            ProcessThread pt = Process.GetCurrentProcess().Threads.Cast<ProcessThread>().Single(t => t.Id == osThreadId);
            long cpuMask = 1L << cpu;
            pt.ProcessorAffinity = new IntPtr(cpuMask);
        }
    }
}