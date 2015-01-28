using System;
using System.Threading;
using System.Threading.Tasks;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace PDNUtils.MultiThreadWorkflow
{
    [RunableClass]
    sealed class MTExecutor
    {
        [Run(0)]
        protected void Run()
        {
            //var mockItemsProvider = new MockItemsProvider(20);
            var mockItemsProvider = new FSItemsProvider("c:\\pdn\\work");

            Action<string> processItem = (item) =>
                                  {
                                      ConsolePrint.print("Item::::::::"+item);
                                      Thread.Sleep(100);
                                  };

            using (var flow = new MultithreadWorkflow<string>(mockItemsProvider, processItem))
            {
                flow.Start();

                bool canContinue = true;

                while (canContinue)
                {
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);

                    switch (consoleKeyInfo.Key)
                    {
                        case ConsoleKey.P:
                            switch (consoleKeyInfo.Modifiers)
                            {
                                case ConsoleModifiers.Alt:
                                    ConsolePrint.print("pausing producer");
                                    flow.PauseProducer();
                                    break;
                                case ConsoleModifiers.Control:
                                    ConsolePrint.print("pausing consumer");
                                    flow.PauseConsumer();
                                    break;
                                default:
                                    ConsolePrint.print("pausing");
                                    flow.Pause();
                                    break;
                            }
                            break;
                        case ConsoleKey.R:
                            switch (consoleKeyInfo.Modifiers)
                            {
                                case ConsoleModifiers.Alt:
                                    ConsolePrint.print("resuming producer");
                                    flow.ResumeProducer();
                                    break;
                                case ConsoleModifiers.Control:
                                    ConsolePrint.print("resuming consumer");
                                    flow.ResumeConsumer();
                                    break;
                                default:
                                    ConsolePrint.print("resuming");
                                    flow.Resume();
                                    break;
                            }
                            break;
                        case ConsoleKey.S:
                            flow.Stop();
                            break;
                        case ConsoleKey.W:
                            canContinue = false;
                            ConsolePrint.print("waiting for...");
                            Task.Factory.StartNew(() =>
                                                      {
                                                          Thread.Sleep(1000);
                                                          flow.Stop();
                                                      });
                            flow.Wait();
                            ConsolePrint.print("finished waiting.");
                            break;
                        case ConsoleKey.Q:
                            canContinue = false;
                            flow.Stop();
                            break;
                    }
                }
            }

            ConsolePrint.print("\n\n\ndone!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n\n\n");
            Console.ReadKey();
        }
    }
}