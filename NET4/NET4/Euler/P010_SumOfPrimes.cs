using System;
using System.Threading;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P010_SumOfPrimes : P007_10001Prime
    {
        [Run(0)]
        protected void SolveIt2()
        {
            ConsolePrint.ShowTime = true;

            byte parallelism = 1;//(byte)Environment.ProcessorCount;

            long p = 1;
            long sum = 0;

            Action<object> action = (__procId) =>
            {
                long prime = 0;
                SetThreadProcessorAffinity((int)__procId);

                while (p < 2000000)
                {
                    long i = Interlocked.Increment(ref p);

                    if (Common.IsPrime(i))
                    {
                        Interlocked.Add(ref sum, i);
                        //DebugFormat("i: {0} prime: {1} pos:{2}", i, IsPrime(i), curpos);
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

            var timer = new Timer(_ => DebugFormat("current p: {0}, sum: {1}", p, sum), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10));

            for (int cpu = 0; cpu < parallelism; cpu++)
            {
                threads[cpu].Join();
            }

            timer.Dispose();

            DebugFormat("sum:{0}", sum);

            Debug("main finished");
        }
    }
}