using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Experimental.IO;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace PDNUtils.MultiThreadWorkflow
{

    [RunableClass]
    internal class TestItems
    {
        [Run(0)]
        protected void Go()
        {
            //init
            var systemTempPath = Path.GetTempPath();
            var testFolder = Path.Combine(systemTempPath, "FSItemsProvider");
            var testFolder2 = Path.Combine(testFolder, "TestFolder2");
            Directory.CreateDirectory(testFolder2);
            var testFile = Path.Combine(testFolder, "testfile");
            var testFile2 = Path.Combine(testFolder2, "testfile2");
            File.WriteAllText(testFile, string.Empty);
            File.WriteAllText(testFile2, string.Empty);

            //
            ConsolePrint.print("1 case.");
            var r = new FSItemsProvider(new[] { testFolder2, testFile }).GetItems(CancellationToken.None);
            foreach (string s in r)
            {
                ConsolePrint.print(s);
            }

            ConsolePrint.print("2 case.");
            r = new FSItemsProvider(new[] { testFolder }).GetItems(CancellationToken.None);
            foreach (string s in r)
            {
                ConsolePrint.print(s);
            }

            // cleanup
            File.Delete(testFile);
            File.Delete(testFile2);
            Directory.Delete(testFolder2);
            Directory.Delete(testFolder);
        }
    }

    public class FSItemsProvider : IItemsProvider<string>
    {
        private readonly IEnumerable<string> paths;

        public FSItemsProvider(string path)
        {
            this.paths = Enumerable.Repeat(path, 1);
        }

        public FSItemsProvider(IEnumerable<string> paths)
        {
            this.paths = paths;
        }

        public IEnumerable<string> GetItems(CancellationToken cancel)
        {
            return LongInnerRecursiveWalk2(paths, cancel);
        }

        protected IEnumerable<string> LongInnerRecursiveWalk2(IEnumerable<string> paths, CancellationToken cancel)
        {
            foreach (string path in paths)
                if (!cancel.IsCancellationRequested)
                {
                    bool exists = false;

                    try
                    {
                        exists = LongPathDirectory.Exists(path);
                    }
                    catch (UnauthorizedAccessException unae)
                    {
                        //log.Error(string.Format("folder [{0}]", path), unae);
                    }

                    if (exists)
                    {
                        IEnumerable<string> pathsBuf = null;
                        try
                        {
                            pathsBuf = LongPathDirectory.EnumerateFiles(path).ToList();
                        }
                        catch (UnauthorizedAccessException unae)
                        {
                            //log.Error(string.Format("folder [{0}]", path), unae);
                        }

                        if (pathsBuf != null)
                            foreach (string s in pathsBuf)
                            {
                                yield return s;
                            }

                        pathsBuf = null;

                        try
                        {
                            IEnumerable<string> dirs = LongPathDirectory.EnumerateDirectories(path).ToList();
                            pathsBuf = LongInnerRecursiveWalk2(dirs, cancel);
                        }
                        catch (UnauthorizedAccessException unae)
                        {
                            //log.Error(string.Format("folder [{0}]", path), unae);
                        }
                        if (pathsBuf != null)
                            foreach (string s in pathsBuf)
                            {
                                yield return s;
                            }

                    }
                    else
                    {
                        bool b = false;
                        try
                        {
                            b = LongPathFile.Exists(path);
                        }
                        catch (UnauthorizedAccessException unae)
                        {
                            //log.Error(string.Format("folder [{0}]", path), unae);
                        }

                        if (b)
                        {
                            yield return path;
                        }
                    }
                }
        }
    }
}
