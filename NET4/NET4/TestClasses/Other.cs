using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Win32;
using PDNUtils.Help;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;
using PDNUtils.Worker;

namespace NET4.TestClasses
{
    [RunableClass]
    class Other : RunableBase
    {

        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Run(0)]
        public static void TestQueueRemove()
        {
            Queue<string> queueOriginal = new Queue<string>();
            queueOriginal.Enqueue("10");
            queueOriginal.Enqueue("9");
            queueOriginal.Enqueue("8");
            queueOriginal.Enqueue("7");
            queueOriginal.Enqueue("6");
            queueOriginal.Enqueue("5");
            queueOriginal.Enqueue("4");
            queueOriginal.Enqueue("3");
            queueOriginal.Enqueue("2");

            ConsolePrint.print(queueOriginal);

            var queueTemporary = new Queue<string>(queueOriginal.Count);

            ConsolePrint.print(queueOriginal);

            while (queueOriginal.Count > 0)
            {
                var item = queueOriginal.Dequeue();
                int res;
                if (int.TryParse(item, out res) && res % 2 == 0)
                {
                    queueTemporary.Enqueue(item);
                }
            }

            while (queueTemporary.Count > 0)
            {
                queueOriginal.Enqueue(queueTemporary.Dequeue());
            }

            ConsolePrint.print(queueOriginal);
        }

        [Run(0)]
        protected void TestVersionClass()
        {
            var v1 = new Version("2011.1.1");
            var v2 = new Version("2011.1.1.200");

            if (v1.Revision == v2.Revision)
            {
                ConsolePrint.print("tada1: {0}", v1);
            }
            else
            {
                v1 = new Version(v1.Major, v1.Minor, v1.Build);
                v2 = new Version(v2.Major, v2.Minor, v2.Build);
                ConsolePrint.print("tada2: {0}", v1);
            }

            if (v1.Major <= 2011 && v1.Minor <= 1 && v1.Build <= 1)
            {
                ConsolePrint.print("tada: {0}", v1);
            }

            ConsolePrint.print("compare: {0}", v1.CompareTo(v2));
        }

        [Run(0)]
        protected static void TestRemoveString()
        {
            string suffix = ".ext";
            string objectId = "aaaa.ext";

            if (objectId.EndsWith(suffix))
            {
                objectId = objectId.Remove(objectId.Length - suffix.Length) + ".EOCMapExtension";
            }

            ConsolePrint.print("[{0}]", objectId);
        }

        [Run(0)]
        private static void TestCrypt()
        {
            string enc = CryptHelper.Encrypt("ыыoř", true);
            string dec = CryptHelper.Encrypt(enc, false);
            Console.WriteLine("encoded [" + enc + "]");
            Console.WriteLine("decoded [" + dec + "]");

            var inStr = "matroska690000000000000произвольный";
            var x = CryptHelper.Encrypt(inStr, true);
            var y = CryptHelper.Encrypt(x, false);
            Console.Out.WriteLine("{0} => {1} => {2}", inStr, x, y);
        }

        [Run(false)]
        private static void ReverseSentence()
        {
            var sentence = "mama myla ramu";
            var arr = sentence.Split(new string[] { " " }, StringSplitOptions.None);
            var cnt = arr.Length;

            StringBuilder res = new StringBuilder();

            for (int i = arr.Length - 1; i >= 0; i--)
            {
                res.Append(arr[i]);
                res.Append(" ");
            }

            Console.Out.WriteLine("res = {0}", res);
        }

        [Run(0)]
        protected static void TryParse()
        {
            int x;
            int.TryParse(null, out x);
            ConsolePrint.print("x={0}", x);
        }

        [Run(0)]
        protected static void Distance()
        {
            Point p1 = new Point(-2, -3);
            Point p2 = new Point(-4, 4);
            double d = Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
            ConsolePrint.print(d);
        }

        [Run(0)]
        private static void StreamTest()
        {
            using (MemoryStream ms = new MemoryStream(Encoding.Default.GetBytes("abc")))
            {
                using (StreamReader sr = new StreamReader(ms))
                {
                    var s = sr.ReadToEnd();
                    Console.Out.WriteLine("s = {0}", s);
                }
            }
        }

        [Run(0)]
        protected void NullableTest()
        {
            int? a = null;
            if (a.HasValue)
            {
                Type t = Nullable.GetUnderlyingType(a.GetType());
            }

            Nullable<int> a2 = null;
        }

        [Run(0)]
        protected void DbNullToString()
        {
            Object n = DBNull.Value;

            ConsolePrint.print(n as string);
            ConsolePrint.print(n.ToString());
        }

        [Run(false)]
        public static void TestSplitCollection()
        {
            int[] a = new int[] { 8, 2, 4, 3, 6, 7, 7, 6, 112, 5, 88 };
            ICollection<ICollection<int>> cc = Utils.SplitCollection<int>(a, 4);
            foreach (ICollection<int> coll in cc)
            {
                ConsolePrint.print(coll);
            }
        }

        [Run(false)]
        public static void TestDNS()
        {
            ConsolePrint.print("hostname: [{0}]", Utils.ResolveIP(null));
            ConsolePrint.print("hostname: [{0}]", Utils.ResolveIP(""));
            ConsolePrint.print("hostname: [{0}]", Utils.ResolveIP("127.0.0.1"));
        }

        [Run(false)]
        private static void TestLatin1()
        {
            string s = "ывва123456latinйййййццууыва!";
            StringBuilder sb = new StringBuilder();
            foreach (char ch in s)
            {
                char tmp = ch;
                if (ch > 127)
                {
                    tmp = '#';
                }
                sb.Append(tmp);
            }
            ConsolePrint.print(sb.ToString());
        }

        [Run(false)]
        public static void TestFN()
        {
            string name = null;
            name = FSHelper.GetNextFileName(name);
            FSHelper.SaveToFileUTF8(name, "some text");

            FileInfo fi = new FileInfo("foo.bar");
            ConsolePrint.print("Name={0}", fi.Name);
        }

        [Run(false)]
        public static void TestNamedCollection()
        {
            IDictionary<string, List<object>> cont = new Dictionary<string, List<object>>();
            List<object> lo = new List<object>();
            lo.Add(5);
            cont["aaa"] = lo;
            cont["aaa"] = lo;
            ROColllection<List<object>> ro = new ROColllection<List<object>>(cont, true);
            foreach (string key in ro)
            {
                object o = ro[key];
                ConsolePrint.print(o);
            }
            lo.Add(500);
            foreach (string key in ro)
            {
                object o = ro[key];
                ConsolePrint.print(o);
            }

            try
            {
                ro.Add("a", lo);                // will throw an exception mofo
            }
            catch (Exception e)
            {
                ConsolePrint.print(e);
                log.Error(e, e);
            }
        }

        [Run(false)]
        public static void TestROCollection()
        {
            IDictionary<object, object> ro = new Dictionary<object, object>();
            ro.Add(721, 721);
            ro.Add(7221, 721);
            ro.Add(7121, 721);
            ro = new ReadOnlyDictionary<object, object>(ro);
            foreach (object o in ro)
            {
                ConsolePrint.print(o);
            }
            try
            {
                ro.Clear();// will throw an exception mofo
            }
            catch (Exception e)
            {
                ConsolePrint.print(e);
                log.Error(e, e);
            }
        }

        [Run(false)]
        public static void TestUrlEnc()
        {
            string decoded = HttpUtility.UrlDecode("C185%5CL350%5C/2nqq2onk.gif");
            ConsolePrint.print("decoded:" + decoded);
            string enc = HttpUtility.UrlEncode("персиковое варенье");
            ConsolePrint.print("encoded:[" + Encoder.URLEncode("| +-/\\ы") + "]");

        }

        public object sync = new object();

        public void longOp1()
        {
            Console.WriteLine("longOp1");
            lock (sync)
            {
                Console.WriteLine("longOp1 - inside lock");
                Thread.Sleep(5000);
            }
            Console.WriteLine("longOp1 - outside lock");
        }

        public void longOp2()
        {
            Console.WriteLine("longOp2");
            lock (sync)
            {
                Console.WriteLine("longOp2 - inside lock");
                Thread.Sleep(5000);
            }
            Console.WriteLine("longOp2 - outside lock");
        }

        [Run(false)]
        public static void TestMath()
        {
            int a = 7 / 6;
            ConsolePrint.print("a = " + a + " b = " + (int)Math.Ceiling(5.0000000000004));
        }

        [Run(false)]
        protected static void TestRegistry()
        {
            ConsolePrint.print(Registry.ClassesRoot.OpenSubKey(".aspx").GetValue("Content Type1"));
        }

        [Run(false)]
        protected static void TestLDAPAuth()
        {
            ConsolePrint.print("auth:" + LDAPAuthHelper.Auth("user@domain", "paSSword11"));
        }

        [Run(0)]
        protected void TestADAuth()
        {
            ADHelper.Validate("denys.pavlov", new string(new char[] { (char)76, (char)111, (char)103, (char)105, (char)116, (char)101, (char)99, (char)104, (char)48, (char)50 }), "TUL");
            ADHelper.Validate("denys.pavlov", new string(new char[] { (char)80, (char)97, (char)115, (char)115, (char)119, (char)111, (char)114, (char)100, (char)48, (char)49 }), "SWDEV");
            ADHelper.Validate("support", "support", "tul");
        }

        [Run(false)]
        protected static void TestPipeConnect()
        {
            string resource = @"\\127.0.0.1";
            ConnectorTool ct = null;
            try
            {
                ct = new ConnectorTool(resource, @"tul\\denys.pavlov", "");
                ct.Connect();
                //Help.ShareConnector.UseRecord(@"\\127.0.0.1\public", "test", "prograMMator99", "pavlov");
                //Help.ShareConnector.Stop(resource, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                if (ct != null)
                {
                    ct.Dispose();
                }
            }
        }

        [Run(false)]
        protected static void TestGetHash()
        {
            var tc = new Other();
            for (int i = 0; i < 10; i++) { tc.SomeHash(); }
        }

        [Run(false)]
        protected static Other TestThreads()
        {
            Other tc = new Other();
            Thread t1 = new Thread(tc.longOp1);
            Thread t2 = new Thread(tc.longOp2);
            t1.Start();
            Thread.Sleep(500);
            t2.Start();
            return tc;
        }

        [Run(false)]
        public static void TryWebConfig()
        {
            WebCfgHelper cfg = new WebCfgHelper();
            ConsolePrint.print("res:" + cfg.AppSettings["size"]);
            ConsolePrint.print("res:" + cfg.ConnectionStrings["default"]);

            Configuration conf = cfg.GetConfig();
            ConsolePrint.print("res*:" + conf.AppSettings.Settings.Count);

            string machineName = System.Environment.MachineName;
            ConsolePrint.print(machineName);
        }

        [Run(false)]
        public static void TestXML()
        {
            string txml = TestXMLMethods.ArrayListOfPairsToXML(TestXMLMethods.GetExamplePairs());
            ConsolePrint.print(txml);
            TestXMLMethods.ParseXML(txml);
        }

        [Run(false)]
        public static void TestPassGen()
        {
            int[] iArr = { 1, 1, 1, 0 };
            char[][] arr = new[]
                               {
                new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x' },
                new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X' },
                new[] { '2', '3', '4', '5', '6', '7', '8', '9' },
                //new[] { '!', '?', '@', '#', '$', '%', '^', '&', '*', '+', '/', '=' }
                               };
            var pg = new PasswordGenerator(iArr, arr, 8, 8);
            for (int i = 0; i < 10; i++)
            {
                ConsolePrint.print("pass:[" + pg.GeneratePassword() + "]");
            }
        }

        public void SomeHash()
        {
            Console.WriteLine(GuidToString());
        }

        public string GuidToString()
        {
            const int max_length = 15;
            bool is_bad_length = true;
            StringBuilder sb = new StringBuilder();
            do
            {
                //generate new guid
                Guid guid = Guid.NewGuid();
                string tmp = guid.ToString();
                byte[] buf = guid.ToByteArray();
                sb.Length = 0;
                for (int i = 0; i < buf.Length; i++)
                {
                    sb.Append(buf[i]);
                }
                if (sb.Length >= max_length)
                    is_bad_length = false;
            } while (is_bad_length);
            sb.Length = 15;
            return sb.ToString();
        }

        [Run(0)]
        protected static void FormatTest()
        {
            string s = get<String>();
            long aa = Convert.ToInt64("0");
            int a = int.Parse("0");
            int? nil = default(int?);
            Console.WriteLine(s + a + aa + nil);
            double d1 = 1;
            double d3 = Math.Round(d1, 2);
            double d2 = 2.0909090909090909090909;
            double d4 = Math.Round(d2, 2);
            Console.WriteLine(string.Format("{0:#.##} - {1:#.##}", d1, d2));
            Console.WriteLine(string.Format("{0} - {1} ", d3, d4));

            ConsolePrint.print(string.Format("[{0}-{1}-{2}]", "a", null, "c"));

            ConsolePrint.print(DateTime.Now.ToString("ddMMyyyy"));
            ConsolePrint.print(DateTime.Now.ToString("yyyyMMdd-HHmmss-fffffff"));
        }

        [Run(0)]
        protected void ParseCustomDateString()
        {
            string dateFormat = "yyyyMMdd-HHmmss-fffffff";
            string dateStr = "20200101-223344-9876543";
            var dt = DateTime.ParseExact(dateStr, dateFormat, null);
            Debug(dt);

            dateStr = "20200101-223344-9876543_sometext";
            dateStr = dateStr.Substring(0, dateFormat.Length);
            bool res = DateTime.TryParseExact(dateStr, dateFormat, null, DateTimeStyles.None, out dt);
            DebugFormat("parsed: {0} value: {1}", dt, res);
        }

        [Run(0)]
        protected static void JoinString()
        {
            ConsolePrint.print(string.Join("*", new int[] { 1 }));
            ConsolePrint.print(string.Join("*", new int[] { 1, 2, 3 }));
        }

        [Run(0)]
        private void StringToBytes()
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] buf;

            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, "abcd");
                buf = ms.ToArray();
            }
        }

        [Run(0)]
        protected static void BytesFormat()
        {
            var sbytes = "FFFE63006F006D006D";

            byte[] buf = new byte[sbytes.Length / 2];

            for (int pos = 0; pos < sbytes.Length - 1; pos += 2)
            {
                string sbuf = string.Format("{0}{1}", sbytes[pos], sbytes[pos + 1]);
                byte b = byte.Parse(sbuf, NumberStyles.HexNumber);
                buf[pos / 2] = b;
            }

            ConsolePrint.print("initial string:{0}", sbytes);
            ConsolePrint.print("bytes:{0}", string.Join(null, buf.Select(b => b.ToString("X2"))));
        }

        [Run(0)]
        protected void StringCollectionTest()
        {
            StringCollection sc = new StringCollection { "a", "b", "e" };

            string format = string.Join("\n", sc);
            ConsolePrint.print(format);

            ConsolePrint.print(sc.ToString());

            foreach (var VARIABLE in sc)
            {
                ConsolePrint.print(VARIABLE);
            }
        }

        [Run(false)]
        protected static void TimeSpanTest()
        {
            TimeSpan span;

            // Initialize a time span to zero.
            span = TimeSpan.Zero;
            Console.WriteLine(span);

            // Initialize a time span to 14 days.
            span = new TimeSpan(-14, 0, 0, 0, 0);
            Console.WriteLine(span);

            // Initialize a time span to 1:02:03.
            span = new TimeSpan(1, 2, 3);
            Console.WriteLine(span);


            // Initialize a time span to 250 milliseconds.
            span = new TimeSpan(0, 0, 0, 0, 250);
            Console.WriteLine(span);

            // Initalize a time span to 99 days, 23 hours, 59 minutes, and 59.999 seconds.
            span = new TimeSpan(99, 23, 59, 59, 999);
            Console.WriteLine(span);

            // Initalize a time span to 3 hours.
            span = new TimeSpan(3, 0, 0);
            Console.WriteLine(span);

            // Initalize a timespan to 25 milliseconds.
            span = new TimeSpan(0, 0, 0, 0, 25);
            Console.WriteLine(span);

            // Initalize a timespan to 25 milliseconds.
            span = new TimeSpan(0, 0, 0, 0, 2500);
            Console.WriteLine(span);
        }

        [Run(false)]
        protected static void TestClipboard()
        {
            var text = "xxx";
            Clipboard.SetText(text);
            var returned = Clipboard.GetText();
            ConsolePrint.print("text:'{0}'", returned);
        }

        [Run(false)]
        public static void TestRandomNum()
        {
            StringBuilder sb = new StringBuilder();

            Random r = new Random();

            for (int i = 0; i < 10; i++)
            {
                string rnd = r.Next(0, 9).ToString();
                sb.Append(rnd);
            }

            Console.Out.WriteLine("cluid:" + sb);
        }

        [Run(false)]
        public static void ListenFS()
        {
            string path = @"..\..\folder_to_listen";
            string filter = "*.xml";

            using (FileSystemWatcher fsWath = new FileSystemWatcher(path, filter))
            {
                fsWath.Changed += fsWathEvent;
                fsWath.IncludeSubdirectories = true;
                fsWath.EnableRaisingEvents = true;
            }
        }

        [Run(false)]
        static void fsWathEvent(object sender, FileSystemEventArgs e)
        {
            Console.Out.WriteLine("path: \"{0}\"\nname: \"{1}\"\ntype: \"{2}\" ", e.FullPath, e.Name, e.ChangeType);
        }

        [Run(false)]
        public static void CopyColl()
        {
            Dictionary<string, CloneableClass> d1 = new Dictionary<string, CloneableClass>();
            d1.Add("a", new CloneableClass(1));

            Dictionary<string, CloneableClass> d2 = new Dictionary<string, CloneableClass>(d1);
            d2["a"] = (CloneableClass)d1["a"].Clone();
            d2["a"].Value = 2;

            Console.WriteLine("d1[\"a\"]=" + d1["a"]);
        }

        [Run(false)]
        public static void readxml()
        {
            XPathNavigator nav;
            XPathDocument docNav;

            // Open the XML.
            using (StreamReader file_reader = new StreamReader(@"d:\WORK\RulesHandler\RulesSimulator\request.xml"))
            {
                using (XmlReader xreader = new XmlTextReader(file_reader))
                {
                    docNav = new XPathDocument(xreader);
                }
            }

            nav = docNav.CreateNavigator();

            XPathExpression expr = nav.Compile("//GetClient2");
            //XPathNodeIterator it = nav.Select(expr);

            XmlNamespaceManager man = new XmlNamespaceManager(new NameTable());
            man.AddNamespace("unicorn", "http://schemas.unicorn.eu/testws");

            object o = nav.Evaluate("//unicorn:GetClient", man);

            XPathNavigator n = nav.SelectSingleNode("//unicorn:GetClient2", man);
            string s = n != null ? n.Value : null;

            n = nav.SelectSingleNode("//unicorn:BirthDate", man);
            s = n.Value;

            n = nav.SelectSingleNode("//unicorn:BirthNumber", man);
            s = n.Value;

            XmlDocument xd = new XmlDocument();
            xd.LoadXml("<a name=\"b\">valueee</a>");

            XmlNode root = xd.DocumentElement;
            if (root.FirstChild != null)
            {
                string ss = root.FirstChild.Value;
            }


            //XmlDocument doc = new XmlDocument();
            //doc.Load(file_reader);
            //file_reader.Close();
            //XmlNode node = doc.SelectSingleNode("//unicorn:CLUID",man);

            //foreach (XPathNavigator node in it)
            //{
            //    Console.WriteLine(node.Name + "::" + node.Value);
            //}

            //XmlElement root = doc.DocumentElement;
            ////XmlNodeList search_node = root.SelectNodes("//*", man);
            //XmlNodeList search_node = root.SelectNodes("//unicorn:GetClient2", man);
            //foreach (XmlNode node in search_node)
            //{
            //    Console.WriteLine(node.Name + "::" + node.Value);
            //    foreach (XmlNode search_node2 in node.ChildNodes)
            //    {
            //        Console.WriteLine(search_node2.Name + "::" + search_node2.Value);
            //    }
            //}

        }

        [Run(false)]
        protected static void FetchFolderName()
        {
            var cacheFolder = @"c:\proj\folder1\folderx";
            string res;
            res = FSHelper.FetchFolderName(@"", cacheFolder);
            ConsolePrint.print("res={0}", res);
            res = FSHelper.FetchFolderName(@"c:\", cacheFolder);
            ConsolePrint.print("res={0}", res);
            res = FSHelper.FetchFolderName(@"c:\proj", cacheFolder);
            ConsolePrint.print("res={0}", res);
            res = FSHelper.FetchFolderName(@"c:\proj\folder1", cacheFolder);
            ConsolePrint.print("res={0}", res);
            res = FSHelper.FetchFolderName(@"c:\proj\folder2", cacheFolder);
            ConsolePrint.print("res={0}", res);
        }

        [Run(false)]
        protected static void TestImages()
        {
            DirectoryWalker walker = new DirectoryWalker(false);
            walker.Walk(new DirectoryInfo("images"),
                fi =>
                {
                    Image newImage = Image.FromFile(fi.FullName);
                });
        }

        [Run(false)]
        protected static void ReformatDirResult()
        {
            var fileName = @"..\..\files.txt";
            var f = new FileInfo(fileName);
            var fout = new FileInfo("rfiles.txt");

            using (var reader = f.OpenText())
            using (var writer = new StreamWriter(fout.OpenWrite()))
            {
                string prefix = null;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    line = line.Trim(' ', '\t');
                    var directoryOf = "Directory of ";

                    if (line.StartsWith(directoryOf))
                    {
                        prefix = line.Substring(directoryOf.Length);
                        //ConsolePrint.print("[{0}]", prefix);
                    }
                    else if (!string.IsNullOrWhiteSpace(line))
                    {
                        Regex r = new Regex(@"\d{2}/\d{2}/\d{4}\s.*");
                        var m = r.Match(line);
                        if (m.Success)
                        {
                            var suffix = line.Substring(39);
                            var fullPath = string.Format(@"{0}\{1}", prefix, suffix);
                            ConsolePrint.print(fullPath);
                            writer.WriteLine(fullPath);
                        }
                    }
                }
            }
        }

        [Run(false)]
        protected static void CheckMaps()
        {
            var walker = new DirectoryWalker();

            var d = new HashSet<string>();

            walker.Walk(new DirectoryInfo("maps"),
                        f =>
                        {
                            using (var reader = f.OpenText())
                            {
                                while (!reader.EndOfStream)
                                {
                                    var line = reader.ReadLine();
                                    line = line.Trim(' ', '\t');

                                    if (!string.IsNullOrWhiteSpace(line))
                                    {
                                        Regex r = new Regex(@"layer\dpicture:\s*""(?<path>.*)""",
                                                            RegexOptions.IgnoreCase);
                                        Match m = r.Match(line);
                                        if (m.Success)
                                        {
                                            GroupCollection gc = m.Groups;
                                            string path = gc["path"].Value;
                                            if (!string.IsNullOrEmpty(path))
                                            {
                                                path = path.Replace(@"\\", @"\");
                                                //ConsolePrint.print("[{0}]", path);
                                                d.Add(path);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                );

            var fout = new FileInfo("usergraphics.files");
            using (var writer = new StreamWriter(fout.OpenWrite()))
            {
                foreach (var path in d)
                {
                    ConsolePrint.print("[{0}]", path);
                    writer.WriteLine(path);
                }
                writer.Flush();
            }

        }

        [Run(false)]
        protected static void ComparePaths()
        {
            var dirfiles = new FileInfo("rfiles.txt");
            var dirFilesSet = new HashSet<string>();

            using (var reader = dirfiles.OpenText())
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    dirFilesSet.Add(line);
                }
            }

            var usergraphics = new FileInfo("usergraphics.files");
            var userGraphicsSet = new HashSet<string>();
            using (var reader = usergraphics.OpenText())
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    userGraphicsSet.Add(line);
                }
            }

            string MSG_OK = "OK    ";
            string MSG_ABSENT = "ABSENT";

            var results = new List<string>();
            foreach (var userGraphicsEntry in userGraphicsSet)
            {
                var msg = dirFilesSet.Contains(userGraphicsEntry) ? MSG_OK : MSG_ABSENT;
                results.Add(string.Format("{0} {1}", msg, userGraphicsEntry));
            }

            results.Sort();

            var fout = new FileInfo("results.txt");
            using (var writer = new StreamWriter(fout.OpenWrite()))
            {
                foreach (var result in results)
                {
                    writer.WriteLine(result);
                }
                writer.Flush();
            }
        }

        [Run(false)]
        protected static void TestStopwatchHelper()
        {
            var stopWatchHelper = new StopWatchHelper("maps.aspx");

            stopWatchHelper.BeginGlobal();
            Thread.Sleep(500);
            stopWatchHelper.EndSection("wrong");
            stopWatchHelper.BeginSection("global");
            Thread.Sleep(50);
            stopWatchHelper.BeginSection("request1");
            stopWatchHelper.BeginSection("request2");
            Thread.Sleep(500);
            stopWatchHelper.EndSection("request1");
            stopWatchHelper.EndSection("request2");
            Thread.Sleep(500);
            stopWatchHelper.EndSection("global");
            Thread.Sleep(50);
            stopWatchHelper.BeginSection("wrong");
            stopWatchHelper.BeginSection("wrong");
            stopWatchHelper.BeginSection(null);
            Thread.Sleep(50);
            stopWatchHelper.EndGlobal();
            var results = stopWatchHelper.Results;
            stopWatchHelper.PrintResults(log);

        }

        [Run(false)]
        protected static void ConcatTest()
        {
            const string test = @"word1 word2 eof
                                  word3";
            ConsolePrint.print("result is: '{0}'", test);
        }

        public static T get<T>() where T : class
        {
            T t = default(T);
            t = string.Empty as T;
            return t;
        }

        [Run(0)]
        protected void TestEmbeddedResource()
        {
            Assembly a = Assembly.GetAssembly(this.GetType());
            var names = a.GetManifestResourceNames();
            ConsolePrint.print(string.Join("\n", names));

            var fileStream = a.GetManifestResourceStream(a.GetName().Name + "." + "TestData.TestMap.EOCMap");

            StreamReader sr = new StreamReader(fileStream);
            ConsolePrint.print(sr.ReadToEnd());
            sr.Close();
        }

        [Run(0)]
        protected void TestFinallyBlock()
        {
            try
            {
                throw new Exception("test");
            }
            finally
            {
                ConsolePrint.print("finally block");
            }

            ConsolePrint.print("this shouldn't have happened!");
        }

        [Run(0)]
        protected void TestObsolete()
        {
            //ObsoleteClass.Do();
        }

        [Run(0)]
        protected void ShowStackTrace()
        {
            var st = new StackTrace(true);
            for (int i = 0; i < Math.Min(st.FrameCount, 4); i++)
            {
                MethodBase methodBase = st.GetFrame(i).GetMethod();
                Type type = methodBase.DeclaringType;
                var s = string.Format("{0}::{1}", (type != null ? (type.Namespace + "." + type.Name) : string.Empty), methodBase.Name);
            }

            st = new StackTrace(2, false);
            for (int i = 0; i < st.FrameCount; i++)
            {
                MethodBase methodBase = st.GetFrame(i).GetMethod();
                Type type = methodBase.DeclaringType;
                var s = string.Format("{0}::{1}", (type != null ? (type.Namespace + "." + type.Name) : "null"), methodBase.Name);
                ConsolePrint.print(s);
            }

            ConsolePrint.print(Environment.StackTrace);
        }

        [Run(0)]
        protected void AllocateArray()
        {
            var intA = new int[100000];
            var objA = new object[100000];
        }

        [Run(0)]
        protected void TestLog4Net()
        {
            Exception inner = new IOException("no such path");
            Exception e = new InvalidOperationException("test eggog", inner);

            log.Error(e);
            log.Error("messaga", e);
            log.Error(e, e);
        }

        [Run(0)]
        protected void TestStackFrame()
        {
            for (int i = 0; i < 20; i++)
            {
                var sf = new StackFrame(i, false);
                var method = sf.GetMethod();

                string s = "at ";

                if (method != null)
                {
                    Type declaringType = method.DeclaringType;
                    s += declaringType.FullName + "::";
                }

                s += method.Name;

                ConsolePrint.print("declaringType {0}:{1}", i, s);
            }
        }

        [Run(0)]
        protected void TestCharOperations()
        {
            string[] strings = new string[]
                                   {
                                       "",
                                       "a",
                                       "b",
                                       "cd",
                                       "xx",
                                       "000"
                                   };

            foreach (var s in strings)
            {
                string buf = ' ' * (8 - s.Length) + s;
                ConsolePrint.print("'{0}' : '{1}'", s, buf);
            }

            char[] reverse = new char[] { 'R', 'E', 'V', 'E', 'R', 'S', 'E' };

            for (int i = 0; i < reverse.Length / 2; i++)
            {
                reverse[i] = (char)(reverse[i] ^ reverse[reverse.Length - 1 - i]);
                reverse[reverse.Length - 1 - i] = (char)(reverse[i] ^ reverse[reverse.Length - 1 - i]);
                reverse[i] = (char)(reverse[i] ^ reverse[reverse.Length - 1 - i]);
            }

            ConsolePrint.print(reverse);
        }

        [Run(0)]
        protected void ConvertGuid()
        {
            string ss = "01234567-0123-3210-0123-0123456789AB";
            //string input = "516BE3DC78A8493B8C53D0B6F9DA34D9";

            Func<string, string> convert = (string input) =>
                                               {
                                                   Guid guid = Guid.Parse(input);
                                                   string[] parts = guid.ToString().Split(new char[] { '-' });

                                                   // part 1
                                                   string stringPart1 = string.Concat(
                                                       string.Concat(parts[0].Reverse()),
                                                       string.Concat(parts[1].Reverse()),
                                                       string.Concat(parts[2].Reverse())
                                                       );

                                                   // part 2

                                                   int groupSize = 2;
                                                   string parts45 = string.Join(string.Empty, parts[3], parts[4]);
                                                   // split into groups by 2 elements
                                                   IEnumerable<IGrouping<int, Tuple<char, int>>> groupsBy2 = parts45.Select((s, pos) => Tuple.Create(s, pos)).GroupBy(i => i.Item2 / groupSize);

                                                   // reverse elements within group and concatenate
                                                   var reversedAndConcatenated = groupsBy2.Select(g => g.Reverse()).Aggregate((g1, g2) => g1.Union(g2)).Select(i => i.Item1);
                                                   var stringPart2 = string.Join(string.Empty, reversedAndConcatenated);

                                                   // compound
                                                   string res = string.Concat(stringPart1, stringPart2);
                                                   return res;
                                               };

            string result = convert(ss);
            string result2 = convert(result);
            ConsolePrint.print("{0} -> {1}", ss, result);
            ConsolePrint.print("{0} -> {1}", result, result2);
        }

        [Run(0)]
        protected void TestBigInt()
        {

            BigInteger bigInteger = BigInteger.Parse("12345678901234567890123456789012345678901234567890");
            var r = BigInteger.Multiply(bigInteger, 2);
            var rr = BigInteger.ModPow(10, 4, 1);
            ConsolePrint.print(bigInteger.ToString());
        }

        [Run(0)]
        protected void TestIsOperator()
        {
            ConsolePrint.print("null is string: {0}", null is string);
        }

        [Run(0)]
        protected void TestContinue()
        {
            int i = 0;

            do
            {
                i++;
                if (i < 3)
                    continue;
                ConsolePrint.print(i);
            } while (i < 5);
        }

        [Run(0)]
        protected void TestExceptionDetails()
        {
            try
            {
                int zero = 0;

                try
                {
                    int z = zero / zero;
                }
                catch (Exception e)
                {
                    var applicationException = new ApplicationException("rethrow", e);
                    // put some hint why exception occured
                    applicationException.Data.Add("divider_value", zero);
                    throw applicationException;
                }
            }
            catch (Exception e)
            {
                var extendedexceptionDetails = PDNUtils.Help.Utils.GetExtendedExceptionDetails(e);
                log.Error("Ordinary:", e);
                log.ErrorFormat("ToString:{0}", e);
                log.ErrorFormat("Detailed:{0}", extendedexceptionDetails);
                ConsolePrint.print(extendedexceptionDetails);
            }
        }
    }

    public class ProgramStateInfo
    {
        public enum x { a, b, c }
        public x xx { get; set; }
        public int count { get; set; }
        public List<string> en { get; set; }
    }


}
