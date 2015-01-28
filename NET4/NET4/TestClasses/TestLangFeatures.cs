using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    static class TestLangFeatures
    {
        private static IDictionary<string, IEnumerable<int>> _dict = new Dictionary<string, IEnumerable<int>>
                                                                  {
                                                                      {"a", new List<int> {10}},
                                                                      {"b", new List<int> {100, 55}},
                                                                  };

        private static IEnumerable<Func<FileInfo, string>> foo = new List<Func<FileInfo, string>>
                                                              {
                                                                  fi=>fi.FullName,
                                                                  fi=>fi.Name
                                                              };

        [Run(0)]
        public static void Go()
        {
            var x = Yield();

            foreach (var i in x)
            {
                Console.Out.WriteLine("name = {0} : pos {1}", i.Name, i.pos);
            }
        }

        [Run(0)]
        static void TestTuple()
        {
            ConsolePrint.print("tuple");
            Tuple<string, string> tuple = Tuple.Create("one", "two");
            ConsolePrint.print(tuple);
            ConsolePrint.print("1:{0}, 2:{1}", tuple.Item1, tuple.Item2);
        }

        [Run(0)]
        static void TestLazy()
        {
            Lazy<int> liDefault = new Lazy<int>();
            ConsolePrint.print("liDefault:{0}", liDefault.Value);

            int count = 0;
            Lazy<int> liFactory = new Lazy<int>(() => count++);
            ConsolePrint.print("liFactory:{0}, count:{1}", liFactory.Value, count);
            ConsolePrint.print("liFactory:{0}, count:{1}", liFactory.Value, count);

        }

        public static void Coll()
        {
            var dict = new Dictionary<string, IEnumerable<int>> {{"a",new List<int>{10}},
                {"b",new List<int>{100,55}},
            };

            var s = string.Join(Environment.NewLine, dict.Select(entry => string.Format("{0} : {1}", entry.Key, ToString<IEnumerable<int>, int>(entry.Value))));

            s = ToString<IDictionary<string, IEnumerable<int>>, string, IEnumerable<int>, int>(dict);

            Console.Out.WriteLine("dict:\n{0}", s);
        }

        public static string ToString<T, TK, TV, TEt>(IDictionary<TK, TV> dict)
            where T : IDictionary<TK, TV>
            where TV : IEnumerable<TEt>
        {
            return string.Join(Environment.NewLine, dict.Select(entry => string.Format("{0} : {1}", entry.Key, ToString<TV, TEt>(entry.Value))));
        }

        public static string ToString<T, TV>(T s) where T : IEnumerable<TV>
        {
            return string.Join(",", s.Select(x => x.ToString()));
        }

        static TestLangFeatures()
        {
            ConsolePrint.print("TestLangFeatures() static ctor");
            Console.Out.WriteLine(ToString<IDictionary<string, IEnumerable<int>>, string, IEnumerable<int>, int>(_dict));
        }

        public static IEnumerable<dynamic> Yield()
        {
            for (int i = 0; i < 10; i++)
            {
                yield return new { Name = "pos" + i, pos = i };
            }

            yield break;
        }

        public static void TestAnonymousTypes()
        {
            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>("key", "value");

            var x = new { kvp.Value, kvp.Key, Pos = 10 };

            Console.Out.WriteLine("x = {0}-{1}-{2}, type={3}", x.Value, x.Key, x.Pos, x.GetType().Name);
        }

        [Run(0)]
        public static void TestCriticalSection()
        {
            var o = new object();

            lock (o)
            {
                try
                {
                }
                finally
                {
                    ConsolePrint.print("inside CS");
                    int x = 0;
                    int a = 10/x;
                    ConsolePrint.print("before exit CS");
                }
            }

            ConsolePrint.print("outside CS");
        }

    }

    public class TestClass
    {
        private readonly dynamic a;

        public TestClass(dynamic a)
        {
            this.a = a;
        }

        public dynamic ResProp()
        {
            return a.Prop1;
        }

        public dynamic ResMethod(dynamic param)
        {
            return a.Method1(param);
        }

        public string NamedParams(string p0, string p1 = "param1", string p2 = "param2")
        {
            return string.Format("p0={0},p1={1},p2={2}", p0, p1, p2);
        }



    }

    public class Executor
    {
        public int Prop1
        {
            get { return 112; }
        }

        public string Method1(string param)
        {
            return param.ToUpper();
        }
    }
}