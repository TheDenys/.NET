using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using PDNUtils.Help;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class LINQTest : RunableBase
    {
        [Run(0)]
        protected void RemoveWorse()
        {
            IEnumerable<Tuple<string, int>> coll = new[]
            {
                Tuple.Create("good",50),
                Tuple.Create("good",10),
                Tuple.Create("bad", 0),
                Tuple.Create("bad", 100),
            };

            // we want remove name duplicates and get: good 50, bad 0
            IDictionary<string, Tuple<string, int>> noDup = new Dictionary<string, Tuple<string, int>>();

            foreach (var tuple in coll)
            {
                Tuple<string, int> tmp;

                if (noDup.TryGetValue(tuple.Item1, out tmp))
                {
                    if (tmp.Item2 < tuple.Item2)
                        noDup[tmp.Item1] = tuple;
                }
                else
                {
                    noDup.Add(tuple.Item1, tuple);
                }
            }

            coll = noDup.Values;
        }

        [Run(0)]
        public static void TestOnList()
        {
            IList<int> l = new List<int>(5);
            l.Add(8);
            l.Add(8);
            l.Add(4);
            l.Add(7);
            l.Add(-10);
            l.Add(28);

            IList<int> l2 = new List<int>(5);
            l2.Add(8);
            l2.Add(800);
            l2.Add(4);
            l2.Add(7);
            l2.Add(-10);
            l2.Add(280);

            IEnumerable<int> positives = from a in l
                                         where a > 0
                                         select a;

            IEnumerable<int> noneq = from a in l
                                     from b in l
                                     where a != b
                                     select a;

            noneq = l.SelectMany(a => l.Where(t => t != a));

            var except = from i in l2
                         let ell = (from ii in l select ii)
                         where !ell.Contains(i)
                         select i;

            ConsolePrint.print("except count: {0}", except.Count());
            ConsolePrint.print(EnumerableToString(except, " "));

            ConsolePrint.print(EnumerableToString(noneq, " "));

            ConsolePrint.print(EnumerableToString(l, " + "));

            Console.Out.WriteLine("positives.Count() = {0}", positives.Count());
        }

        public static string EnumerableToString<T>(IEnumerable<T> coll, string separator)
        {
            if (coll == null)
            {
                return null;
            }
            string res = String.Join(separator, coll.Select(a => a.ToString()).ToArray());
            return res;
        }

        [Run(0)]
        protected void TestOnDictionary()
        {
            var testDict = new Dictionary<string, IEnumerable<string>>
                               {
                                   {"first group",  new string[]{"A1","A2","B1"}},
                                   {"second group", new string[]{"A31","A32","B2"}},
                                   {"third group",  new string[]{"A21","A22","B3"}},
                                   {"fourth group", new string[]{"A41","A42","B4"}},
                                   {"fifth group",  new string[]{"A421","A422","C5"}},
                               };

            var res = from d in testDict
                      let l = (from v in d.Value where v.StartsWith("B") select v)
                      where l.Count() > 0
                      select l;

            res.ForEach(v => ConsolePrint.print(v), false);
            //ConsolePrint.print("res:{0}", res);
        }

        [Run(0)]
        public static void TestXmlLinq()
        {
            string[] source = new string[]
                {
                    "first group,10",
                    "second group,10",
                    "third group,100"
            };
            XElement xElement =
                new XElement("root",
                     from str in source
                     let fields = str.Split(',')
                     select new XElement("Group",
                         new XAttribute("GroupName", fields[0]),
                         new XElement("members", fields[1])
                     )
                );

            Console.Out.WriteLine("xml:" + Environment.NewLine + xElement);
        }

        [Run(0)]
        public static void Go()
        {
            IEnumerable<string> en1 = new List<string> { "vasja", "petja" };
            IEnumerable<string> en2 = new List<string> { "ljuda", "alina" };
            IEnumerable<string> en3 = new List<string> { "karina", "kristina" };

            var cartesian = from n1 in en1 from n2 in en2 from n3 in en3 select n1 + '-' + n2 + '-' + n3;

            Console.Out.WriteLine("cartesian = \n{0}", string.Join("\n", cartesian));

            Console.Out.WriteLine("indexes = {0}", string.Join("\n", cartesian.Select((s, index) =>
                string.Format("combination {0} : {1}", index, s))));

            var groups = from c in cartesian
                         group c by c.Substring(0, c.IndexOf("-"));

            var groupsSel = from c in cartesian
                            group c by c.Substring(0, c.IndexOf("-")) into g
                            select new { G = g.Key, Coll = g };

            var groupsSelMethod = cartesian.GroupBy(c => c.Substring(0, c.IndexOf("-"))).Select(g => g.ToDictionary(i => i.GetHashCode(), i => i));

        }

        [Run(0)]
        protected static void ContainersPrototype()
        {
            var coll = new Container[]
                           {
                               new Container {uid = "1", Status = Status.Down},
                               new Container {uid = "1", Status = Status.Unknown},
                               new Container {uid = "1", Status = Status.Down},
                               new Container {uid = "2", Status = Status.Down},
                               new Container {uid = "2", Status = Status.Down},
                               new Container {uid = "2", Status = Status.Down},
                               new Container {uid = "4", Status = Status.Unknown},
                           };

            var res = coll.GroupBy(c => c.uid).ToDictionary(g => g.Key, g => g.Select(c => c).ToList());//.Select(g => g.ToDictionary(c => g.Key, c => g.Select(i => i)));

            ConsolePrint.print(res);

        }

        [Run(0)]
        protected static void GroupLINQ()
        {
            var coll = new List<Container>
                           {
                               new Container
                                   {
                                       uid = "x",
                                       IPAddresses = new List<string>{"1","3","5"}
                                   },
                               new Container
                                   {
                                       uid = "y",
                                       IPAddresses = new List<string>{"2","4","6"}
                                   },
                           };

            var res = from c in coll
                      from ip in c.IPAddresses
                      select new { c.uid, ip };

            ConsolePrint.print("res:" + res);
        }

        public static void ReadXML()
        {
            FileInfo fi = new FileInfo("NPM.Template.Meru.Controller.xml");
            using (Stream stream = fi.OpenRead())
            {
                XElement xel = XElement.Load(stream);
                Console.Out.WriteLine("xel.Name = {0}", xel.Name);
                var props = from el in xel.Elements("Device").Elements("Property") select el;
                Console.Out.WriteLine("props = {0}", string.Join("\n", props.Select(p => p.Name + " " + p.Attribute(XName.Get("name")))));
            }
        }

        /// <summary>
        /// this is for spizdislov project
        /// </summary>
        [Run(0)]
        public static void LangPairs()
        {
            var compoundCmd = @"mkdir {0}-{1}
pushd {0}-{1}
wget --tries=2 --no-clobber --timeout=40 --recursive --level=3 http://wordsteps.com/{0}/{1}/vocabulary/div0/ || echo ""{0}-{1} failed!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!""
popd
";
            var langs = new string[] { "en", "fr", "ru", "es", "zh", "de", "ja", "it", "pt", "fi", "he" };
            Array.Sort(langs);

            var pairsGroups = from l1 in langs
                              from l2 in langs
                              where l1 != l2
                              group l2 by l1;

            foreach (var pairsGroup in pairsGroups)
            {
                var key = pairsGroup.Key;

                var f = new FileInfo(Path.ChangeExtension(key, "bat"));

                using (var stream = new StreamWriter(f.OpenWrite()))
                {
                    foreach (var item in pairsGroup)
                    {
                        var cmd = string.Format(compoundCmd, key, item);
                        stream.WriteLine(cmd);
                        ConsolePrint.print(cmd);
                    }
                }
            }
        }

        [Run(0)]
        protected void TestSelectForStatus()
        {
            IEnumerable<Container> coll1 = new Container[] { null, new Container { Status = Status.Unknown }, };

            var unknownState = coll1.Contains(null) || coll1.Where(i => i.Status == Status.Unknown).Count() > 0;

        }

        [Run(0)]
        protected void TestSkip()
        {
            var coll = new[] { 5, 7, 1, 9, 4, -1, 3, 5, 2 };
            var t1 = coll.SkipWhile(i => i > 0).ToArray();
            var t2 = coll.Skip(10).ToArray();
            var t3 = coll.ToList();
            var idx = t3.FindIndex(i => i < 0) + 1;
            var res = t3[idx % t3.Count];

            ConsolePrint.print(t1);
            ConsolePrint.print(t2);
        }

        [Run(0)]
        protected void TestPos()
        {
            var col = new[] { "aa", "bb", "cc", "mm" };
            var col2 = col.Select((s, pos) => new Tuple<string, int>(s, pos));
            ConsolePrint.print(col2);
        }

        [Run(0)]
        protected void RemoveDuplicatesWithDescOrder()
        {
            int[] source = { 7, 4, 1, 3, 9, 8, 6, 7, 2, 1, 8, 15, 8, 23 };
            var res = (from i in source orderby i descending select i).Distinct();
            ConsolePrint.print(res);

            var res2 = source.Distinct().OrderByDescending(i => i);
            ConsolePrint.print(res2);
        }

        [Run(0)]
        protected void TestStringSubst()
        {
            string input = "This Is {0} A tEsT {1} strinG";
            string res = Garble(input);
            ConsolePrint.print(res);
        }

        private static string Garble(string input)
        {
            if (input == null)
            {
                return input;
            }

            const string alphabet_orig = "abcdefghijklmnopqrstuvwxyz";
            const string alphabet_subst = "zʎxʍʌnʇsɹbdouɯlʞɾıɥƃɟǝpɔqɐ";

            List<char> alphabetOriginalList = alphabet_orig.ToCharArray().ToList();

            Func<char, int> GetPos = (ch) =>
            {
                return alphabetOriginalList.IndexOf(char.ToLowerInvariant(ch));
            };

            var reverted = input.ToCharArray().Reverse();
            var garbled = reverted.Select(
                ch =>
                {
                    var pos = GetPos(ch);
                    var pos_subst = (alphabet_subst.Length - 1) - pos;
                    switch (ch)
                    {
                        case '{':
                            return '}';
                        case '}':
                            return '{';
                        default:
                            return pos != -1 && pos_subst < alphabet_subst.Length ? alphabet_subst[(alphabet_subst.Length - 1) - pos] : ch;
                    }
                });

            string res = string.Join(null, garbled);
            return res;
        }

        [Run(0)]
        protected void SplitByCount()
        {
            int groupSize = 2;

            DataTable table = new DataTable("t1");
            table.Columns.Add("c1");

            for (int i = 0; i < 15; i++)
            {
                DataRow row = table.NewRow();
                row[0] = i;
                table.Rows.Add(row);
            }

            IEnumerable<DataRow> eRows = table.AsEnumerable();

            IEnumerable<IGrouping<int, Tuple<object, int>>> groups = eRows.Select((r, pos) => Tuple.Create(r[0], pos)).GroupBy(i => i.Item2 / groupSize);

            foreach (var g in groups)
            {
                ConsolePrint.print("g:{0} - {1}", g.Key, g.Count());

                foreach (var tuple in g)
                {
                    ConsolePrint.print("{0}-{1}", tuple.Item1, tuple.Item2);
                }
            }

        }

        private enum Status
        {
            Unknown,
            Unreachable,
            Down,
            Up,
            Warning
        }

        private class Container
        {
            public string uid { get; set; }
            public List<string> IPAddresses { get; set; }
            public Status Status { get; set; }
        }

    }
}