using System;
using System.Threading;
using System.Threading.Tasks;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class LazyTest
    {

        private int counter;

        private Lazy<string> lazyString;

        private readonly object sync = new object();

        public LazyTest()
        {
            lazyString = new Lazy<string>(ThreadSafeFactory, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        [Run(0)]
        protected void ThreadSafeFactoryMethodCall()
        {
            Task[] tasks = new Task[21];

            for (int i = 0; i < 11; i++)
            {
                tasks[i] = new Task(() =>
                                        {
                                            try
                                            {
                                                ConsolePrint.print("value={0}", lazyString.Value);
                                            }
                                            catch (Exception ex)
                                            {
                                                ConsolePrint.print(ex);
                                            }
                                        });
            }

            for (int i = 0; i < 11; i++)
            {
                tasks[i].Start();
            }

            for (int i = 0; i < 11; i++)
            {
                tasks[i].Wait();
            }
        }

        private string ThreadSafeFactory()
        {
            //lock (sync)
            {
                if (lazyString.IsValueCreated) { return lazyString.Value; }

                int i = counter++;
                if (i < 10)
                {
                    return null;
                    //throw new InvalidOperationException("counter is less than 10");
                }

                return i + " done!!!";
            }
        }
    }
}