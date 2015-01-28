using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Threading;

namespace PDNUtils.Help
{
    /// <summary>
    /// prints to console
    /// </summary>
    public static class ConsolePrint
    {

        private static volatile bool showTime;

        public static bool ShowTime
        {
            get { return showTime; }
            set { showTime = value; }
        }

        public static void printMap(string s1, string s2)
        {
            ConsoleWriteLine("[{0}]=>[{1}]", s1, s2);
        }

        public static void print(string format, params object[] ss)
        {
            ConsoleWriteLine(format, ss);
        }

        public static void print()
        {
            _print(string.Empty);
        }

        public static void print(string s)
        {
            _print((object)s);
        }

        public static void print(object o)
        {
            if (o == null)
            {
                _print("OBJECT IS NULL");
            }
            else if (o is NameValueCollection)
            {
                _print(o as NameValueCollection);
            }
            else if (o is IEnumerable)
            {
                _print(o as IEnumerable);
            }
            else
            {
                _print(o);
            }
        }

        private static void _print(object o)
        {
            ConsoleWriteLine(o);
        }

        private static void _print(IEnumerable en)
        {
            StringBuilder sb = new StringBuilder("[");
            var list = new ArrayList();
            foreach (var v in en)
            {
                list.Add(v);
            }
            sb.Append(string.Join(",", list.ToArray()));
            sb.Append("]");
            ConsoleWriteLine(sb.ToString());
        }

        private static void _print(NameValueCollection nv)
        {
            StringBuilder sb = new StringBuilder("[");
            foreach (string s in nv.Keys)
            {
                sb.Append("\"");
                sb.Append(s);
                sb.Append("\"=\"");
                sb.Append(nv.Get(s));
                sb.Append("\",");
            }
            CutTailing(sb);
            sb.Append("]");
            ConsoleWriteLine(sb.ToString());
        }

        private static void CutTailing(StringBuilder sb)
        {
            if (sb.Length > 1)
            {
                sb.Length--;
            }
        }

        private static void ConsoleWriteLine(object o)
        {
            Console.WriteLine(GetTime() + o);
        }

        private static void ConsoleWriteLine(string s)
        {
            Console.WriteLine(GetTime() + s);
        }

        private static void ConsoleWriteLine(string fmt, params object[] parameters)
        {
            Console.WriteLine(GetTime() + fmt, parameters);
        }

        private static string GetTime()
        {
            return showTime ? string.Format("[{0}] ({1:000}) ", DateTime.Now.ToString("HH:mm:ss.fffffff"), Thread.CurrentThread.ManagedThreadId) : string.Empty;
        }

    }
}
