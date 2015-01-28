using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using PDNUtils.Config;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;
using PDNUtils.Serialization;

namespace NET4.TestClasses
{
    [RunableClass]
    public class SerializeTest
    {
        [Run(0)]
        private static void testSerializeWCFStyle()
        {
            TestSerializeWCF.test();
        }

        [Run(0)]
        protected static void TestSerializeXml()
        {
            var testObj = new ProgramStateInfo { xx = ProgramStateInfo.x.b, count = 4 };
            testObj.en = new List<string> { "1", "2", "x" };
            string ser = testObj.XmlSerialise();
            ConsolePrint.print(ser);
            var deserTestObj = ser.XmlDeserialise<ProgramStateInfo>();
            ConsolePrint.printMap(testObj.xx.ToString(), deserTestObj.xx.ToString());
            ConsolePrint.printMap(testObj.count.ToString(), deserTestObj.count.ToString());
        }

        [Run(false)]
        protected static void TestSerializeToFile()
        {
            var testObj = new ProgramStateInfo { xx = ProgramStateInfo.x.b, count = 4 };
            testObj.en = new List<string> { "1", "2", "x" };
            ProgramStateManager<ProgramStateInfo>.Instance.Save(testObj);
            var deserTestObj = ProgramStateManager<ProgramStateInfo>.Instance.Load();
            ConsolePrint.printMap(testObj.xx.ToString(), deserTestObj.xx.ToString());
            ConsolePrint.printMap(testObj.count.ToString(), deserTestObj.count.ToString());
            ConsolePrint.print(deserTestObj.en);
        }

        [Run(0)]
        protected static void TestSerializeXmlCollections()
        {
            var testObj = new ClassWithCollections();
            testObj.CustomProperty.Add(
                new Dictionary<string, object>
                    {
                        { "1", "x" },
                        { "2", "y" },
                        { "3", "z" },
                    }
            );
            testObj.Properties.Add("prop1", DateTime.Now);

            using (Stream memStream = new MemStreamWrapper())
            {
                testObj.Serialize(memStream);
                memStream.Seek(0, SeekOrigin.Begin);
                var deserTestObj = SerializeHelper.Deserialize<ClassWithCollections>(memStream);
                ConsolePrint.print(deserTestObj.CustomProperty);
                ConsolePrint.print(deserTestObj.Properties);

                memStream.Seek(0, SeekOrigin.Begin);
                var sr = new StreamReader(memStream);
                var xml = sr.ReadToEnd();
                ConsolePrint.print(xml);
            }

            DataTable dt = null;
            try
            {
                dt = new DataTable("tableX");
                dt.Columns.Add("col1");
                dt.Rows.Add("val1");

                FileStorageMock fsMock = new FileStorageMock(new Type[] { typeof(string[]) });
                fsMock.Add("abc", testObj);
                fsMock.Add("kandeljabr <> bomba", testObj);
                fsMock.Add("scape'\"", testObj);
                fsMock.Add("_8*%$", testObj);
                fsMock.Add("table_one", dt);
                ClassWithCollections ttt = fsMock.Get<ClassWithCollections>("abc");
                ttt = fsMock.Get<ClassWithCollections>("kandeljabr <> bomba");
                ttt = fsMock.Get<ClassWithCollections>("scape'\"");
                ttt = fsMock.Get<ClassWithCollections>("_8*%$");
                dt = fsMock.Get<DataTable>("table_one");

            }
            finally
            {
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }

        public class ClassWithCollections
        {
            private Dictionary<string, object> properties = new Dictionary<string, object>();
            private List<Dictionary<string, object>> customProperty = new List<Dictionary<string, object>>();
            public Dictionary<string, object> Properties
            {
                get
                {
                    return properties;
                }
            }

            public IList<Dictionary<string, object>> CustomProperty
            {
                get
                {
                    return customProperty;
                }
            }
        }

        public class MemStreamWrapper : Stream
        {
            private MemoryStream memStream = new MemoryStream();

            public override void Flush()
            {
                memStream.Flush();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return memStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                memStream.SetLength(value);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return memStream.Read(buffer, offset, count);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                memStream.Write(buffer, offset, count);
            }

            public override bool CanRead
            {
                get { return memStream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return memStream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return memStream.CanWrite; }
            }

            public override long Length
            {
                get { return memStream.Length; }
            }

            public override long Position
            {
                get { return memStream.Position; }
                set { memStream.Position = value; }
            }

            protected override void Dispose(bool disposing)
            {
            }
        }
    }
}