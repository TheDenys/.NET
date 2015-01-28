using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using PDNUtils.Help;
using PDNUtils.IO;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{

    [RunableClass]
    public class PathTest
    {
        [Run(0)]
        public void TestSearch()
        {
            PathUtils.RollFilenames(".", "banana", 5);
            PathUtils.RollFilenames(".", "foo.bar", 6);

            File.CreateText("banana").Close();
            File.CreateText("foo.bar").Close();

            var fn = Directory.GetFiles(".", "foo.bar.*");
            ConsolePrint.print(fn);
            fn = Directory.GetFiles(".", "banana.*");
            ConsolePrint.print(fn);

        }

        [Run(0)]
        public void TestSort()
        {
            string[] files = new string[] { ".\\foo.bar.1", ".\\foo.bar.2", ".\\foo.bar" };
            PathUtils.CompareNumericExtensions("foo.bar", files);

            ConsolePrint.print(files);
        }

    }

    public class FileSearch
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DirectoryInfo dir;
        private IPatternSearcher ps;

        public FileSearch(string dir, IPatternSearcher ps)
        {
            this.dir = new DirectoryInfo(dir);
            if (!this.dir.Exists)
            {
                throw new InvalidOperationException("no such directory");
            }
            this.ps = ps;
        }

        public LinkedList<FileInfo> GetFiles()
        {
            LinkedList<FileInfo> l = new LinkedList<FileInfo>();
            _GetFiles(dir, l);
            return l;
        }

        private void _GetFiles(DirectoryInfo di, LinkedList<FileInfo> l)
        {
            FileInfo[] arr_fi = di.GetFiles();
            foreach (FileInfo fi in arr_fi)
            {
                if (_IsContains(fi, ps))
                {
                    l.AddLast(fi);
                }
            }
            DirectoryInfo[] arr_di = di.GetDirectories();
            foreach (DirectoryInfo di_sub in arr_di)
            {
                _GetFiles(di_sub, l);
            }
        }

        private static bool _IsContains(FileInfo fi, IPatternSearcher pattern)
        {
            return pattern.IsMatch(fi);
        }

    }

    public interface IPatternSearcher
    {
        bool IsMatch(FileInfo fi);
    }

    public class ExcludePatternSearcher : IPatternSearcher
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Regex regexp;
        private string exclude;
        public ExcludePatternSearcher(Regex regexp, string exclude)
        {
            this.regexp = regexp;
            this.exclude = exclude;
        }

        public bool IsMatch(FileInfo fi)
        {
            bool bres = false;
            string src = null;

            using (StreamReader sr = new StreamReader(fi.FullName))
            {
                src = sr.ReadToEnd();
            }

            string str_lower = exclude.ToLower();
            MatchCollection mc = regexp.Matches(src);
            foreach (Match m in mc)
            {
                bool is_in = m.Value.ToLower().IndexOf(str_lower) == -1;
                log.Debug(m.Value);
                if (is_in)
                {
                    log.Debug("it's here:" + m.Value);
                    bres = true;
                    break;
                }
            }
            return bres;
        }
    }

}
