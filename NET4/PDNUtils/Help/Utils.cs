using PDNUtils.Worker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace PDNUtils.Help
{
    public static class Utils
    {

        /// <summary>
        /// uses Dns.GetHostEntry to perform reverse dns resolution
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string ResolveIP(string ip)
        {
            if (String.IsNullOrEmpty(ip))
            {
                return ip;
            }
            IPAddress ipaddr = null;
            IPAddress.TryParse(ip, out ipaddr);
            IPHostEntry he = Dns.GetHostEntry(ipaddr);
            string host = he != null ? he.HostName : ip;
            return host;
        }

        /// <summary>
        /// splits one big collection to several collections with limited length
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static ICollection<ICollection<T>> SplitCollection<T>(ICollection<T> coll, int size)
        {
            int rows = (int)Math.Ceiling((double)coll.Count / size);
            int lastCount = coll.Count % size;
            T[] tmpArr = new T[coll.Count];
            T[] buf = new T[size];
            coll.CopyTo(tmpArr, 0);
            int copyCount = size;

            ICollection<ICollection<T>> res = new List<ICollection<T>>();
            for (int i = 0; i < rows; i++)
            {
                if (i == rows - 1)
                {
                    copyCount = lastCount;
                    buf = new T[copyCount];
                }
                Array.Copy(tmpArr, i * size, buf, 0, copyCount);
                res.Add(new List<T>(buf));
            }

            return res;
        }

        /// <summary>
        /// cuts the drive letter from a path
        /// if path doesn't contain drive letter will throw FormatException
        /// for unc path returns \\
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetDriveLetter(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentException("path can't be empty or null");
            }

            if (path[1] == Path.VolumeSeparatorChar)
            {
                return path[0].ToString().ToUpperInvariant();
            }
            else if (path.StartsWith("\\"))//UNC path
            {
                return "\\";
            }

            throw new FormatException("path doesn't contain drive letter information");
        }

        /// <summary>
        /// Generic wrapper for Enum.Parse
        /// </summary>
        /// <typeparam name="T">enum type</typeparam>
        /// <param name="value">value to be parsed</param>
        /// <returns>enum instance</returns>
        public static T GetEnumObject<T>(object value)
        {
            return (T)Enum.Parse(typeof(T), (string)value);
        }

        public static long GetFilesCount(IEnumerable<DirectoryInfo> paths, int? depth, Func<string, bool> pathExcludePredicate)
        {
            long files = 0;
            var dirs = paths.Select(d => d.FullName);
            Action<string> fCounter = fi => Interlocked.Increment(ref files);
            var walker = new LongDirectoryWalker(dirs, fCounter, true, null, pathExcludePredicate);
            walker.LongWalk();
            return files;
        }

        public static long GetFilesCount(string path, int? depth)
        {
            long files = 0;
            var walker = new DirectoryWalker(true);
            walker.Walk(new DirectoryInfo(path), fi => Interlocked.Increment(ref files));
            return files;
        }

        public static string GetDirectoryName(string path)
        {
            int idx = path.LastIndexOf('\\');
            return path.Substring(0, idx);
        }

        /// <summary>
        /// This utility method can be used for retrieving extra details from exception objects.
        /// </summary>
        /// <param name="e">Exception.</param>
        /// <param name="indent">Optional parameter. String used for text indent.</param>
        /// <returns>String with as much details was possible to get from exception.</returns>
        public static string GetExtendedExceptionDetails(object e, string indent = null)
        {
            // we want to be robust when dealing with errors logging
            try
            {
                var sb = new StringBuilder(indent);
                // it's good to know the type of exception
                sb.AppendLine("Type: " + e.GetType().FullName);
                // fetch instance level properties that we can read
                var props = e.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead);

                foreach (PropertyInfo p in props)
                {
                    try
                    {
                        var v = p.GetValue(e, null);

                        // in case of Fault contracts we'd like to know what Detail contains
                        if (e is FaultException && p.Name == "Detail")
                        {
                            sb.AppendLine(string.Format("{0}{1}:", indent, p.Name));
                            sb.AppendLine(GetExtendedExceptionDetails(v, "  " + indent));// recursive call
                        }
                        // Usually this is InnerException
                        else if (v is Exception)
                        {
                            sb.AppendLine(string.Format("{0}{1}:", indent, p.Name));
                            sb.AppendLine(GetExtendedExceptionDetails(v as Exception, "  " + indent));// recursive call
                        }
                        // some other property
                        else
                        {
                            sb.AppendLine(string.Format("{0}{1}: '{2}'", indent, p.Name, v));

                            // Usually this is Data property
                            if (v is IDictionary)
                            {
                                var d = v as IDictionary;
                                sb.AppendLine(string.Format("{0}{1}={2}", " " + indent, "count", d.Count));
                                foreach (DictionaryEntry kvp in d)
                                {
                                    sb.AppendLine(string.Format("{0}[{1}]:[{2}]", " " + indent, kvp.Key, kvp.Value));
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        //swallow or log
                    }
                }

                //remove redundant CR+LF in the end of buffer
                sb.Length = sb.Length - 2;
                return sb.ToString();
            }
            catch (Exception exception)
            {
                //log or swallow here
                return string.Empty;
            }
        }

        public static string GetString(object o)
        {
            return GetStringInternal(o);
        }

        private static string GetStringInternal(object o)
        {
            if (o == null) return "null";
            if (o is string) return (string)o;
            if (ReflectionHelper.HasToString(o)) return o.ToString();
            if (o is IEnumerable) return GetStringInternal(o as IEnumerable);

            return GetStringInternal(o);
        }

        private static string GetStringInternal(IEnumerable e)
        {
            StringBuilder sb = new StringBuilder("[");
            sb.Append(string.Join(",", e.Cast<object>().Select(GetStringInternal)));
            sb.Append("]");
            return sb.ToString();
        }
    }
}
