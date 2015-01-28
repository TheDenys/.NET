using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace PDNUtils.Serialization
{
    public class FileStorageMock
    {

        private static readonly Encoding encoding = Encoding.UTF8;

        public const string FILE_NAME_PREFIX = "sm_";//sm - storage mock

        public const string FILE_NAME = FILE_NAME_PREFIX + "storage.xml";

        private object _SyncRoot = new object();

        private string fileName;

        private readonly Type[] knownTypes;

        public FileStorageMock()
            : this(".", null)
        {
        }

        public FileStorageMock(Type[] knownTypes)
            : this(".", knownTypes)
        {
        }

        public FileStorageMock(string directory, Type[] knownTypes)
        {
            var di = new DirectoryInfo(directory);

            if (!di.Exists)
            {
                di.Create();
            }

            fileName = Path.Combine(di.FullName, FILE_NAME);

            CreateRoot(fileName);

            this.knownTypes = knownTypes;
        }

        private void CreateRoot(string fileName)
        {
            if (File.Exists(fileName) && (new FileInfo(fileName)).Length > 0)
            {
                return;
            }

            using (var f = File.Create(fileName))
            {
                XmlWriter w = XmlWriter.Create(f);
                w.WriteStartElement("root");
                w.WriteEndElement();
                w.Close();
            }
        }

        public void Add<T>(string key, T value)
        {
            Add(new KeyValuePair<string, T>(key, value));
        }


        public void Add<T>(KeyValuePair<string, T> entry)
        {
            lock (_SyncRoot)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName);
                bool hasKey = _GetValueNodeByKey(doc, entry.Key) != null;
                if (hasKey)
                {
                    return;
                }
                XmlNode root = doc.SelectSingleNode("root");
                root.AppendChild(BuildEntry(doc, entry));
                doc.Save(fileName);
            }
        }

        public T Get<T>(string key)
        {
            T t = _GetValue<T>((doc) => _GetValueNodeByKey(doc, key));
            return t;
        }

        public T Get<T>(int hash)
        {
            T t = _GetValue<T>((doc) => _GetValueNodeByHash(doc, hash));
            return t;
        }

        private XmlNode BuildEntry<T>(XmlDocument doc, KeyValuePair<string, T> entry)
        {
            var n = doc.CreateElement("entry");
            BuildKeyChild(doc, n, "key", entry.Key);
            BuildValueChild(doc, n, "hash", entry.Key.GetHashCode().ToString());

            if (typeof(T) == typeof(DataTable))
            {
                string tableXmlSchema = StreamUtils.WriteStreamToString((stream) =>
                {
                    var dt = entry.Value as DataTable;
                    dt.WriteXmlSchema(stream);
                });
                BuildSchemaChild(doc, n, "schema", tableXmlSchema);

                string tableXml = StreamUtils.WriteStreamToString((stream) =>
                {
                    var dt = entry.Value as DataTable;
                    dt.WriteXml(stream);
                });
                BuildValueChild(doc, n, "value", tableXml);
            }
            else
            {
                string serialize = entry.Value.Serialize(knownTypes);
                BuildValueChild(doc, n, "value", serialize);
            }

            return n;
        }

        private XmlNode BuildKeyChild(XmlDocument doc, XmlNode parent, string name, string value)
        {
            var n = doc.CreateElement(name);
            parent.AppendChild(n);
            var cdata = doc.CreateCDataSection(value);
            n.AppendChild(cdata);
            return n;
        }

        private XmlNode BuildSchemaChild(XmlDocument doc, XmlNode parent, string name, string value)
        {
            var n = doc.CreateElement(name);
            parent.AppendChild(n);
            var s = doc.CreateCDataSection(value);
            n.AppendChild(s);
            return n;
        }

        private XmlNode BuildValueChild(XmlDocument doc, XmlNode parent, string name, string value)
        {
            var n = doc.CreateElement(name);
            parent.AppendChild(n);
            n.InnerXml = value;
            return n;
        }

        private XmlNode _GetValueNodeByKey(XmlDocument doc, string key)
        {
            var rootEntryKeyValue = string.Format("root/entry/key");
            XmlNodeList nodes = doc.SelectNodes(rootEntryKeyValue);
            return (from XmlNode node in nodes where node.InnerText == key select node.SelectSingleNode("../value")).FirstOrDefault();
        }

        private XmlNode _GetValueNodeByHash(XmlDocument doc, int hash)
        {
            var rootEntryKeyValue = string.Format("root/entry/hash");
            XmlNodeList nodes = doc.SelectNodes(rootEntryKeyValue);
            return (from XmlNode node in nodes where node.Value == hash.ToString() select node.SelectSingleNode("../value")).FirstOrDefault();
        }

        private T _GetValue<T>(Func<XmlDocument, XmlNode> getNodeFunc)
        {
            lock (_SyncRoot)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName);
                XmlNode valueNode = getNodeFunc(doc);
                string xml = valueNode.InnerXml;

                if (typeof(T) == typeof(DataTable))
                {
                    var schemaNode = valueNode.SelectSingleNode("../schema");
                    string xmlSchema = schemaNode.InnerText;
                    var dt = new DataTable();
                    StreamUtils.ReadStringAsStream(xmlSchema, encoding, dt.ReadXmlSchema);
                    StreamUtils.ReadStringAsStream(xml, encoding, (stream) => dt.ReadXml(stream));
                    return (T)(object)dt;
                }
                else
                {
                    return SerializeHelper.DeserializeFromXmlSnippet<T>(xml, knownTypes);
                }
            }
        }

    }
}