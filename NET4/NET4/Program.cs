using System;
using PDNUtils.Runner;

namespace NET4
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [STAThread]
        static void Main()
        {
            log.Debug("started");

            Action<string> infoAction = name => Console.WriteLine("\n=== executing {0} ===\n", name);

            var runner = new Runner(infoAction, MessageHandlerBuilder.BuildConsoleMessageHandler());
            runner.OnError += Console.WriteLine;
            runner.ExecuteMethods();

            Console.WriteLine("press enter to exit");
            Console.ReadLine();
        }

    }
}
