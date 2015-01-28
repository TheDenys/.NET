using System;
using System.Configuration;
using System.IO;
using PDNUtils.Help;
using PDNUtils.Serialization;

namespace PDNUtils.Config
{
    public class ProgramStateManager<T> where T : class,new()
    {
        private const string PS_KEY = "program.state";

        private static readonly string PS_PATH = ConfigurationManager.AppSettings[PS_KEY];

        private static readonly object locker = new object();

        private static readonly ProgramStateManager<T> instance = new ProgramStateManager<T>();

        public static ProgramStateManager<T> Instance
        {
            get { return instance; }
        }

        protected ProgramStateManager()
        {
            if (string.IsNullOrWhiteSpace(PS_PATH))
            {
                throw new InvalidOperationException("Path to program state is not defined. Check [" + PS_KEY +
                                                    "] key in config file.");
            }
            lock (locker)
            {
                var fi = new FileInfo(PS_PATH);

                // if config doesn't exist we create it
                if (!fi.Exists)
                {
                    var instance = Activator.CreateInstance<T>();
                    InnerSave(instance);
                }
            }
        }

        public T Load()
        {
            lock (locker)
            {
                var fi = new FileInfo(PS_PATH);

                using (var sr = new StreamReader(fi.OpenRead()))
                {
                    var ser = sr.ReadToEnd();
                    var deserTestObj = ser.XmlDeserialise<T>();
                    if (deserTestObj == null)
                    {
                        throw new InvalidOperationException(string.Format("Can't read program state. Check \"{0}\" file.", PS_PATH));
                    }
                    return deserTestObj;
                }

            }
        }

        public void Save(T ps)
        {
            InnerSave(ps);
        }

        private static void InnerSave(T ps)
        {
            lock (locker)
            {
                var fi = new FileInfo(PS_PATH);

                using (var sw = fi.CreateText())
                {
                    string ser = ps.XmlSerialise();
                    sw.Write(ser);
                    sw.Flush();
                }
            }
        }
    }

}