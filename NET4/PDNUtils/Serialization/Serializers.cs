using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using log4net;

namespace PDNUtils.Serialization
{
    /// <summary>
    /// Extension methods for serialization
    /// </summary>
    public static class Serializers
    {

        private static readonly Dictionary<Type, XmlSerializer> serialisers = new Dictionary<Type, XmlSerializer>();

        /// <summary>Serialises an object of type T in to an xml string</summary>
        /// <typeparam name="T">Any class type</typeparam>
        /// <param name="objectToSerialise">Object to serialise</param>
        /// <returns>A string that represents Xml, empty oterwise</returns>
        public static string XmlSerialise<T>(this T objectToSerialise) where T : class, new()
        {
            XmlSerializer serialiser;

            var type = typeof(T);
            if (!serialisers.ContainsKey(type))
            {
                serialiser = new XmlSerializer(type);
                serialisers.Add(type, serialiser);
            }
            else
            {
                serialiser = serialisers[type];
            }

            string xml;
            using (var writer = new StringWriter())
            {
                serialiser.Serialize(writer, objectToSerialise);
                xml = writer.ToString();
            }

            return xml;
        }

        /// <summary>Deserialises an xml string in to an object of Type T</summary>
        /// <typeparam name="T">Any class type</typeparam>
        /// <param name="xml">Xml as string to deserialise from</param>
        /// <returns>A new object of type T is successful, null if failed</returns>
        public static T XmlDeserialise<T>(this string xml) where T : class, new()
        {
            XmlSerializer serialiser;

            var type = typeof(T);
            if (!serialisers.ContainsKey(type))
            {
                serialiser = new XmlSerializer(type);
                serialisers.Add(type, serialiser);
            }
            else
            {
                serialiser = serialisers[type];
            }

            T newObject;

            using (var reader = new StringReader(xml))
            {
                try { newObject = (T)serialiser.Deserialize(reader); }
                catch { return null; } // Could not be deserialized to this type.
            }

            return newObject;
        }

    }

    public static class SerializeHelper
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Encoding Encoding = Encoding.UTF8;

        public static void Serialize<T>(this T objectForSerialization, String fileName)
        {
            try
            {
                using (Stream stream = new FileStream(fileName, FileMode.Create))
                {
                    Serialize(objectForSerialization, stream);
                }
            }
            catch (SecurityException e)
            {
                log.Error("The caller does not have the required permission.", e);
            }
            catch (FileNotFoundException e)
            {
                log.Error("The file cannot be found.", e);
            }
            catch (DirectoryNotFoundException e)
            {
                log.Error("The specified path is invalid, such as being on an unmapped drive.", e);
            }
            catch (PathTooLongException e)
            {
                log.Error("The specified path, file name, or both exceed the system-defined maximum length.", e);
            }
            catch (UnauthorizedAccessException e)
            {
                log.Error("Access is deined. Probably file is locked by another process.", e);
            }
            catch (IOException e)
            {
                log.Error("Probably the stream has been closed.", e);
            }
            catch (Exception e)
            {
                log.Error(e, e);
            }
        }

        public static string Serialize<T>(this T objectForSerialization, Type[] knownTypes)
        {
            string s = StreamUtils.WriteStreamToString((stream) =>
            {
                var ser = new DataContractSerializer(typeof(T), knownTypes);
                XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding);
                ser.WriteObject(writer, objectForSerialization);
                writer.Flush();
            });

            return s;
        }

        public static void Serialize<T>(this T objectForSerialization, Stream stream)
        {
            if (objectForSerialization == null)
            {
                throw new ArgumentNullException("objectForSerialization");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            //#if DEBUG
            //            // this writer allows generate indented xml
            //            var writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = Encoding, Indent = true });
            //            log.Warn("Warning!!! XmlWriter is being used for serialization! Performance is suffered!");
            //#else
            //#endif

            var ser = new DataContractSerializer(typeof(T));

            try
            {
                using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding))
                {
                    ser.WriteObject(writer, objectForSerialization);
                }
            }
            catch (InvalidDataContractException e)
            {
                log.Error(
                    "The type " + typeof(T) +
                    " does not conform to data contract rules. For example, the DataContractAttribute attribute has not been applied to the type.",
                    e);
            }
            catch (SerializationException e)
            {
                log.Error("There is a problem with the instance being written.", e);
            }
            catch (QuotaExceededException e)
            {
                log.Error("The maximum number of objects to serialize has been exceeded. Check the MaxItemsInObjectGraph property.", e);
            }
            catch (InvalidOperationException e)
            {
                log.Error("A call is made to write more output after Close has been called or the result of this call is an invalid XML document.", e);
            }
            catch (Exception e)
            {
                log.Error(e, e);
            }
        }

        public static T Deserialize<T>(String fileName)
        {
            T deserializedObject = default(T);

            try
            {
                using (Stream stream = new FileStream(fileName, FileMode.Open))
                {
                    deserializedObject = Deserialize<T>(stream);
                }
            }
            catch (SecurityException e)
            {
                log.Error("The caller does not have the required permission.", e);
            }
            catch (FileNotFoundException e)
            {
                log.Error("The file cannot be found.", e);
            }
            catch (DirectoryNotFoundException e)
            {
                log.Error("The specified path is invalid, such as being on an unmapped drive.", e);
            }
            catch (PathTooLongException e)
            {
                log.Error("The specified path, file name, or both exceed the system-defined maximum length.", e);
            }
            catch (UnauthorizedAccessException e)
            {
                log.Error("Access is deined. Probably file is locked by another process.", e);
            }
            catch (IOException e)
            {
                log.Error("Probably the stream has been closed.", e);
            }
            catch (Exception e)
            {
                log.Error(e, e);
            }

            return deserializedObject;
        }

        public static T DeserializeFromXmlSnippet<T>(string xml, Type[] knownTypes)
        {
            var ser = new DataContractSerializer(typeof(T), knownTypes);
            T obj = default(T);
            byte[] buf = Encoding.GetBytes(xml);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(buf, new XmlDictionaryReaderQuotas());
            obj = (T)ser.ReadObject(reader);
            return obj;
        }

        public static T Deserialize<T>(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var ser = new DataContractSerializer(typeof(T));
            T obj = default(T);

            try
            {
                using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(stream, Encoding, new XmlDictionaryReaderQuotas(), null))
                {
                    obj = (T)ser.ReadObject(reader);
                }
            }
            catch (SerializationException e)
            {
                log.Error("There is a problem with the instance being read.", e);
            }
            catch (Exception e)
            {
                log.Error(e, e);
            }

            return obj;
        }

    }
}