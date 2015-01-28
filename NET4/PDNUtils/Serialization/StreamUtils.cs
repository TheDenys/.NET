using System;
using System.IO;
using System.Text;

namespace PDNUtils.Serialization
{
    public static class StreamUtils
    {
        public static string WriteStreamToString(Action<Stream> action)
        {
            string s;
            MemoryStream stream = null;
            StreamReader reader = null;

            try
            {
                stream = new MemoryStream();
                action(stream);
                stream.Seek(0, SeekOrigin.Begin);
                reader = new StreamReader(stream);
                s = reader.ReadToEnd();
            }
            finally
            {
                if (reader != null) { reader.Dispose(); }
                if (stream != null) { stream.Dispose(); }
            }

            return s;
        }

        public static void ReadStringAsStream(string xml, Encoding encoding, Action<Stream> action)
        {
            byte[] buf = encoding.GetBytes(xml);
            MemoryStream stream = null;

            try
            {
                stream = new MemoryStream(buf);
                action(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }
    }
}