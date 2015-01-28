using System;
using System.IO;
using System.Linq;
using System.Threading;
using LightIndexer.Config;
using LightIndexer.Indexing;
using log4net;
using PDNUtils.Help;
using PDNUtils.Worker;

namespace LightIndexerConsole
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            log.Debug("started");

            //new TestIndexing().TestIndexSearch();
            //return;

            string argsStr = string.Join(" ", args);
            Console.Out.WriteLine("argsStr = {0}", argsStr);

            if (args.Length > 0 && string.Compare(args[0], "-r", true) == 0)
            {
                string path = @"c:\";
                if (args.Length > 1)
                {
                    path = args[1];
                }

                bool finish = false;
                do
                {
                    Console.Out.Write("Path [{0}] will be indexed. Agree? (y/n/a):", new DirectoryInfo(path).FullName);
                    string answer = Console.ReadLine();
                    switch (answer.Trim().ToLowerInvariant())
                    {
                        case "a":
                            Console.Out.Write("Path:");
                            path = Console.ReadLine();
                            if (Directory.Exists(path))
                            {
                                long files = Utils.GetFilesCount(path, null);
                                Console.Out.WriteLine("files: {0}", files);

                                continue;// in while
                            }
                            else
                            {
                                goto case "a";
                            }
                        case "y":
                            IndexingFacade.IndexWrite(path, Show, null);
                            goto default;
                        default:
                            finish = true;
                            break;
                    }
                } while (!finish);

            }

            Search();

            Console.ReadLine();
        }

        private static void Search()
        {
            Console.Clear();
            var numdoc = Configurator.GetDefaultIndexManager().GetOpenIndexReader().NumDocs();
            Console.Out.WriteLine("total docs in index: {0}", numdoc);

            do
            {
                Console.Out.Write(">");
                string input = Console.ReadLine();
                SearchInIndex(input);
            } while (true);
        }

        private static void SearchInIndex(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.Out.WriteLine("input query");
            }
            else
            {
                var search = new SearchOptions { SearchString = input };
                var res = IndexingFacade.SearchInIndex(search);

                if (res!=null && res.Any())
                {
                    Console.Out.WriteLine(string.Join("\n", res));
                }

                Console.Out.WriteLine("Files found: {0}", res.LongCount());
            }
        }

        private static void Show(DirectoryWalker walker, long total)
        {
            while (walker.State == WalkerState.Clean || walker.State == WalkerState.Running || walker.State == WalkerState.Stopping)
            {
                Console.Clear();
                Console.Out.WriteLine("Processed {0} of {1}", walker.ItemsProcessed, total);
                Thread.Sleep(1500);
            }
            Console.Clear();
        }

    }
}
