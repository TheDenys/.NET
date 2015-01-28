using System.IO;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    class TestXMLMethods
    {

        public static ArrayList GetExamplePairs()
        {
            ArrayList al = new ArrayList();
            for (int i = 0; i < 5; i++)
            {
                TextBox tbLeft = new TextBox();
                TextBox tbRight = new TextBox();
                tbLeft.Text = "col_a_" + i;
                tbRight.Text = "col_b_" + i;
                Pair pair = new Pair(tbLeft, tbRight);
                al.Add(pair);
            }
            return al;
        }

        public static string ArrayListOfPairsToXML(ArrayList al)
        {
            StringBuilder sb = new StringBuilder(100);
            StringWriter twriter = new StringWriter(sb);
            XmlWriter writer = new XmlTextWriter(twriter);
            writer.WriteStartElement("pairs");
            string first, second;
            foreach (Pair pair in al)
            {
                first = ((TextBox)pair.First).Text;
                second = ((TextBox)pair.Second).Text;
                writer.WriteStartElement("pair");
                writer.WriteStartElement("a");
                writer.WriteValue(first);
                writer.WriteEndElement();
                writer.WriteStartElement("b");
                writer.WriteValue(second);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            return sb.ToString();
        }

        public static ArrayList ParseXML(string data)
        {
            ArrayList al = new ArrayList();
            TextReader treader = new StringReader(data);
            XmlReader reader = new XmlTextReader(treader);
            treader = new StringReader(data);
            XmlDocument doc = new XmlDocument();
            doc.Load(treader);
            XmlNodeList l = doc.GetElementsByTagName("pair");
            foreach (XmlNode node in l)
            {
                ConsolePrint.print(node.Name + " " + node.Value);
                al.Add(FetchFromNode(node));
            }
            return al;
        }

        private static Pair FetchFromNode(XmlNode node)
        {
            Pair pair = new Pair();
            XmlNodeList nl = node.ChildNodes;
            string val;
            foreach (XmlNode child in nl)
            {
                TextBox tb = new TextBox();
                if (child.HasChildNodes)
                {
                    val = child.FirstChild.Value;
                    tb.Text = val;
                    if (child.Name == "a")
                    {
                        pair.First = tb;
                    }
                    if (child.Name == "b")
                    {
                        pair.Second = tb;
                    }
                }
            }
            return pair;
        }

        [Run(0)]
        public static void TestXmlAttributeAppend()
        {
            string initial = "<root><pair>yummy</pair></root>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(initial);
            doc.FirstChild.Attributes.Append(doc.CreateAttribute("mixLeft"));
            doc.FirstChild.Attributes["mixLeft"].Value = "bibi";
            string res = doc.OuterXml;
            ConsolePrint.print("initial={0}", initial);
            ConsolePrint.print("res={0}", res);
        }

        [Run(0)]
        protected static void XLinqXml()
        {
            XDocument xDoc = new XDocument();
            xDoc.Add(new XElement("root"));
            var root = xDoc.Root;

            root.Add(
                new XElement[]
                    {
                        new XElement("node1", new XElement("child")), 
                        new XElement("node2"), 
                    });

            var els = root.Elements();
            var desc = root.Descendants();

            ConsolePrint.print(xDoc.ToString());
        }

    }
}
