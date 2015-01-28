using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace NET4.TestClasses
{
    public class TestSerializeWCF
    {

        private enum StreamType
        {
            Memory,
            File
        }

        private static StreamType streamType = StreamType.File;

        private static byte[] _buf;

        private const string FILENAME = "test.ser";

        public static void test()
        {
            try
            {
                WriteObject();
                ReadObject();

            }
            catch (Exception e)
            {
                Console.Out.WriteLine("msg:{0}\ne.StackTrace = {1}", e.Message, e.StackTrace);
            }
        }

        private static Containers mock()
        {
            Containers list = new Containers();

            for (int i = 0; i < 15; i++)
            {
                list.Add(new Container() { Name = string.Format("name {0}", i), Width = 55 + 2 * i });
            }

            return list;
        }

        private static void WriteObject()
        {
            Containers c = mock();

            using (Stream fs = _getStreamOut())
            {
                XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs);
                DataContractSerializer ser = new DataContractSerializer(typeof(Containers));
                ser.WriteObject(writer, c);
                writer.Close();
                fs.Close();
            }

            Console.Out.WriteLine("written");
        }

        private static void ReadObject()
        {
            Containers c = null;

            using (Stream fs = _getStreamIn())
            {
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                DataContractSerializer ser = new DataContractSerializer(typeof(Containers));
                c = (Containers)ser.ReadObject(reader);
                reader.Close();
                fs.Close();
            }
            Console.Out.WriteLine("read");

            Console.Out.WriteLine("content \n{0}", c);
        }

        private static Stream _getStreamOut()
        {
            switch (streamType)
            {
                case StreamType.File:
                    return new FileStream(FILENAME, FileMode.Create);
                case StreamType.Memory:
                    var stream = new AdvancedMemoryStream();
                    stream.OnClose += new OnClose(stream_OnClose);
                    return stream;
            }

            throw new InvalidOperationException();
        }

        static void stream_OnClose(AdvancedMemoryStream sender)
        {
            if (_buf != null)
            {
                return;
            }
            byte[] buffer = new byte[sender.Length];
            sender.Seek(0, SeekOrigin.Begin);
            sender.Read(buffer, 0, (int)(sender.Length - 1));
            _buf = buffer;
        }

        private static Stream _getStreamIn()
        {
            switch (streamType)
            {
                case StreamType.File:
                    return new FileStream(FILENAME, FileMode.Open);
                case StreamType.Memory:
                    return new MemoryStream(_buf);
            }

            throw new InvalidOperationException();
        }

    }

    public class AdvancedMemoryStream : MemoryStream
    {

        public event OnClose OnClose;

        public void InvokeOnClose()
        {
            OnClose handler = OnClose;
            if (handler != null) handler(this);
        }

        public override void Close()
        {
            InvokeOnClose();
            base.Close();
        }
    }

    public delegate void OnClose(AdvancedMemoryStream sender);
}