using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using log4net;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class TestRegex
    {

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [Run(0)]
        public static void TestReplaceNewLines()
        {
            var r = "1"+Environment.NewLine+Environment.NewLine+"0sdfdsf sdsd "+Environment.NewLine+"5 5  5";
            ConsolePrint.print("'{0}' ==> '{1}'", r, Regex.Replace(Regex.Replace(r, "(\\n+)|(\\r+)|(\\s+)", " "), "\\s+", " "));
        }


        [Run(0)]
        public static void TestReplaceIpv6()
        {
            var r = "1:::0";
            ConsolePrint.print("{0} ==> {1}", r, Regex.Replace(r, ":{3,}", "::"));
        }


        [Run(false)]
        public static void TestRegReplace()
        {
            string str_in = "<div> " + Environment.NewLine + " <table style=\"1\" id=\"clid\" > " + Environment.NewLine + " </table> " + Environment.NewLine + " </div> <table> " + Environment.NewLine + "</table> </div>";
            Regex r = new Regex("(<div>\\s*)((.|\n)*.*<table.*[^>]id=\"clid\")((.|\n)*?</table>)((.|\n)*?)</div>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            MatchCollection mc = r.Matches(str_in);
            ConsolePrint.print(mc.Count);
            foreach (Match m in mc)
            {
                ConsolePrint.print("[" + m.Value + "]");
            }
            ConsolePrint.print("repl:" + Environment.NewLine + r.Replace(str_in, "$2$3$4"));
        }

        [Run(false)]
        public static void TestRegexReplace()
        {
            Regex r = new Regex("^[eE]{0,1}[1-9]{1}[0-9]{0,3}$");
            string[] s = {
                             "e",
                             "e0",
                             "e1",
                             "90",
                             "e01",
                             "e902",
                             "E4",
                             "ER",
                             "0900",
                             "90e",
                             "180"
                         };
            foreach (string test in s)
            {
                Console.Out.WriteLine(String.Format("{0} : {1}", test, r.IsMatch(test)));
            }

            string output = null;
            string input = "<a attr1=\"attt\" attr2=\"00\" attr3=\"99\" encoding=\"iso-8859-2\" encoding2=\"iso-8859-2\">";

            output = Regex.Replace(input, "(encoding=\").*?(\")", "$1@IGNORED@$2");

            Console.Out.WriteLine("out:" + output);

            input = "<sadf> <sadfsdfsdf> <applicationid>909</applicationid> </sdfsdf>";
            Regex rParse = new Regex("<applicationid>(.*?)</applicationid>");
            Match m = rParse.Match(input);
        }

        [Run(0)]
        public static void TestRegParseSwisUri()
        {
            string map_to = null;
            Regex r = new Regex("^swis://./Orion/Orion.Groups/ContainerID=(?<containerId>\\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            string addr = "swis://./Orion/Orion.Groups/ContainerID=591";
            Match m = r.Match(addr);
            ConsolePrint.print("ismatch:" + m.Success);
            if (m.Success)
            {
                GroupCollection gc = m.Groups;
                ConsolePrint.print(gc["containerId"]);
                string year = gc["containerId"].Value;
                map_to = String.Format("Id={0}", year);
            }
            ConsolePrint.print("map_to:[" + map_to + "]");
        }

        [Run(false)]
        public static void TestRegParse()
        {
            string map_to = null;
            Regex r = new Regex("~/articles/(?<year>\\d{2})/(?<month>\\d{2})/(?<day>\\d{2})", RegexOptions.IgnoreCase);
            string addr = "~/articles/08/11/22";
            Match m = r.Match(addr);
            ConsolePrint.print("ismatch:" + m.Success);
            if (m.Success)
            {
                GroupCollection gc = m.Groups;
                ConsolePrint.print(gc["year"] + "." + gc["month"] + "." + gc["day"]);
                string year = gc["year"].Value;
                string month = gc["month"].Value;
                string day = gc["day"].Value;
                map_to = String.Format("pages/Articles.aspx?year={0}&month={1}&day={2}", year, month, day);
            }
            ConsolePrint.print("map_to:[" + map_to + "]");
        }

        [Run(false)]
        public static void TestRegSplit()
        {
            Regex r = new Regex("[\\\\/]", RegexOptions.IgnoreCase);
            //string addr = "~/articles/08/11/22";
            string addr = "~\\articles\\08\\11\\22/sd/dsdsad";
            string[] groups = r.Split(addr);
            foreach (string s in groups)
            {
                ConsolePrint.print("[" + s + "]");
            }
        }


        [Run(false)]
        public static void TryRegex()
        {
            string str_search = "AssociatedConrtolID";
            string str_lower = str_search.ToLower();
            string src = Environment.NewLine + "\n<asp:Label ID=\"bla\" bla>-+-+--<asp:Label ID=\"bla\" AssociatedConrtolID=\"blalla\" bla> 98+98+88+98<br /></asp:Label>,<br /></asp:Label>" + Environment.NewLine + "</asp:Label>" + Environment.NewLine;
            Regex r = new Regex("<(asp:Label|\\w+:localizablelabel).+?>", RegexOptions.IgnoreCase);
            MatchCollection mc = r.Matches(src);
            foreach (Match m in mc)
            {
                bool is_in = m.Value.ToLower().IndexOf(str_lower) != 0;
                log.Debug(m.Value);
                if (is_in)
                {
                    log.Debug("it's here");
                }
            }
            log.Debug("matches: " + mc.Count);
        }

        [Run(false)]
        public static void TestParseQS()
        {
            //ConsolePrint.print(VirtualPathUtility.ToAbsolute("../ElsWww","http://localhost"));
            string[] tst = new string[]{@"http://hostName/test/GetElement.ashx?param=pampam",
                                        @"http://hostName/test/GetElement.ashx?param=pampam/",
                                        @"/test/els/C/\1.jpg",
                                        @"/hostName/test/els/aaa",
                                        @"test/GetElement.ashx?param=pampam",
                                        @"%imagePath%images/asdasd/C/\1.jpg",
                                        @"%imagePath%images/asdasd/C/sdf/1.jpg",
                                        @"%imagePath%images/C1.jpg"};
            foreach (string s in tst)
            {
                var address = ParseAddress(@"http://hostName/test", s);
                ConsolePrint.print(address);
            }
        }

        private enum LinkType
        {
            Undefined,
            Local,
            Image
        }

        private enum PathType
        {
            Image,
            GetElement,
            Folder
        }

        private static string GetRelative(Uri uri)
        {
            //string sHost = uri.GetComponents(UriComponents.Host, UriFormat.SafeUnescaped);
            string sPathRel = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
            //sPathRel = "/" + sHost + "/" + sPathRel;
            sPathRel = "/" + sPathRel;
            return sPathRel;
        }

        [Run(0)]
        protected void ParseUserNames()
        {
            var login = "a\\b";
            ConsolePrint.print("{0} : {1}", login, ParseLogin(login));
            login = "a/b";
            ConsolePrint.print("{0} : {1}", login, ParseLogin(login));
            login = "user@domain";
            ConsolePrint.print("{0} : {1}", login, ParseLogin(login));
            login = "ab";
            ConsolePrint.print("{0} : {1}", login, ParseLogin(login));
            login = ".\\b";
            ConsolePrint.print("{0} : {1}", login, ParseLogin(login));
            login = "./b";
            ConsolePrint.print("{0} : {1}", login, ParseLogin(login));
            login = "uname";
            ConsolePrint.print("{0} : {1}", login, ParseLogin(login));
            login = "uname\\";
            ConsolePrint.print("{0} : {1}", login, ParseLogin(login));
            login = "uname@";
            ConsolePrint.print("{0} : {1}", login, ParseLogin(login));
            login = "uname.aaa@f.q.d.n";
            ConsolePrint.print("{0} : {1}", login, ParseLogin(login));
            login = "ab\\\\ba";
            ConsolePrint.print("{0} : {1}", login, ParseLogin(login));

            do
            {
                Console.Write("login>");
                login = Console.ReadLine();
                ConsolePrint.print("{0} : {1}", login, ParseLogin(login));
            } while ("q" != login);
        }

        public class CredentialsBag
        {
            public string Domain { get; set; }
            public string Username { get; set; }

            public CredentialsBag(string username) : this(null, username) { }

            public CredentialsBag(string domain, string username)
            {
                Domain = domain;
                Username = username;
            }

            public override string ToString()
            {
                return string.Format("domain:{0},user:{1}.", Domain, Username);
            }
        }

        public static CredentialsBag ParseLogin(string login)
        {
            const string pattern = @"(^(?<domain>[^\\@]+)\\{1}(?<user>[^\\@]+)$)|(^(?<user>[^\\@]+)@{1}(?<domain>[^\\@]+)$)|(^(?<username>[^\\@]+)$)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            Match m = regex.Match(login);
            bool success = m.Success;

            if (success)
            {
                GroupCollection gc = m.Groups;
                ConsolePrint.print("domain:'{0}',user:'{1}',username:'{2}'", gc["domain"], gc["user"], gc["username"]);

                if (gc["username"].Success)
                {
                    return new CredentialsBag(gc["username"].Value);
                }
                else
                {
                    return new CredentialsBag(gc["domain"].Value, gc["user"].Value);
                }
            }

            return null;
        }

        public static string ParseAddress(string baseUrl, string address)
        {
            string phPath = null;

            LinkType linkType = LinkType.Undefined;
            PathType pathType = PathType.Folder;
            baseUrl = VirtualPathUtility.RemoveTrailingSlash(baseUrl);
            Uri uriLocal = new Uri(baseUrl);
            string sPathRel = GetRelative(uriLocal);

            // classify path
            string sPrefixFull = baseUrl;// prefix for full specified url
            string sPrefixRel = sPathRel;// prefix for relative url
            string sPrefixImages = @"%imagePath%images";

            string sPathPrefixEls = @"/els/";
            string sPathPrefixGetEl = @"/GetElement.ashx\?";
            string sPathPrefixOther = @"/";

            string sLocalFormat = @"^(?<prefix>{0}|{1}|{2})(?<afterprefix>{3}|{4}|{5})(?<path>.*[^/]$)";// only local urls and path with %imagePath%iamges satisfy this regexp 
            sLocalFormat = String.Format(sLocalFormat, sPrefixFull, sPrefixRel, sPrefixImages, sPathPrefixEls, sPathPrefixGetEl, sPathPrefixOther);
            Regex rxLocal = new Regex(sLocalFormat, RegexOptions.IgnoreCase);
            Match m = Regex.Match(address, sLocalFormat, RegexOptions.IgnoreCase);
            bool succ = m.Success;
            ConsolePrint.print("[" + address + "] ismatch : " + succ);
            if (succ)// only local urls passes and path with %imagePath%iamges passes here
            {
                GroupCollection gc = m.Groups;
                ConsolePrint.print(gc["prefix"] + " = " + gc["afterprefix"] + " = " + gc["path"]);
                string prefix = gc["prefix"].Value;
                string afterprefix = gc["afterprefix"].Value;
                string path = gc["path"].Value;
                //define link type
                linkType = Regex.Match(prefix, sPrefixImages).Success ? LinkType.Image : LinkType.Local;// only 2 possibilitities here
                //define path type
                switch (linkType)
                {
                    case LinkType.Image:
                        pathType = PathType.Image;
                        break;
                    case LinkType.Local:
                        pathType = Regex.Match(afterprefix, sPathPrefixGetEl).Success ? PathType.GetElement : PathType.Folder;
                        break;
                }
                // get physical path
                phPath = GetPhysicalPath(pathType, path);
            }
            return phPath;
        }

        private static string GetPhysicalPath(PathType pathType, string path)
        {
            string res = null;
            string base_path = null;
            switch (pathType)
            {
                case PathType.GetElement:
                    res = GE2Path(path);
                    break;
                case PathType.Folder:
                    base_path = @"d:\data\els";
                    res = FSHelper.CombineAndFixPath(base_path, path);
                    break;
                case PathType.Image:
                    base_path = @"d:\data\els\images";
                    res = FSHelper.CombineAndFixPath(base_path, path);
                    break;
            }
            return res;
        }

        private static string GE2Path(string path)
        {
            string res = "GetElement";
            ConsolePrint.printMap(path, res);
            return res;
        }

        [Run(false)]
        public static void TestSearch()
        {
            FileInfo fi_res = new FileInfo("files.txt");
            fi_res.Delete();

            Regex r = new Regex("<(asp:Label|\\w+:localizablelabel).+?>", RegexOptions.IgnoreCase);
            string exclude = "associatedcontrolid";
            IPatternSearcher ps = new ExcludePatternSearcher(r, exclude);
            FileSearch fs = new FileSearch(@"D:\WORK\Els\Els\ElsWww\", ps);
            LinkedList<FileInfo> l = fs.GetFiles();

            StreamWriter sw = new StreamWriter(fi_res.FullName);
            foreach (FileInfo f in l)
            {
                sw.WriteLine(f.FullName);
                Other.log.Debug(f.Name);
            }
            sw.Close();
        }

        [Run(false)]
        public static void TestIEVer()
        {
            string data = "IE 5.5";

            int res = -1;
            Regex rxCheckIE = new Regex("IE\\s*(?<ver>.*)", RegexOptions.IgnoreCase);
            Regex rxIEVer = new Regex("(?<num>\\d+)\\..*");
            Match m = rxCheckIE.Match(data);
            if (m.Success)
            {
                Group g = m.Groups["ver"];
                Console.WriteLine("gruppen: " + g.Value);
                m = rxIEVer.Match(g.Value);
                if (m.Success)
                {
                    g = m.Groups["num"];
                    Console.WriteLine("gruppen: " + g.Value);
                    int.TryParse(g.Value, out res);
                }
            }
        }

    }
}