using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Experimental.IO;
using log4net;

namespace PDNUtils.IO
{
    public static class ZipHelper
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal static Tuple<string, string> SplitName(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            string zipName = null;
            string packedName = null;
            int idx = fileName.IndexOf(Path.AltDirectorySeparatorChar);
            if (idx == -1) return null;
            zipName = fileName.Substring(0, idx);
            var startIndex = idx + 1;
            if (startIndex >= fileName.Length) return null;
            packedName = fileName.Substring(startIndex, fileName.Length - startIndex);
            return Tuple.Create(zipName, packedName);
        }

        public static IEnumerable<Tuple<string, Stream>> GetFileFromZip(string zipFileName, int maxFileSize)
        {
            var t = SplitName(zipFileName);
            
            if (t == null)
                return null;
            
            return GetFilesFromZip(t.Item1, maxFileSize, t.Item2);
        }

        public static IEnumerable<Tuple<string, Stream>> GetFilesFromZip(string zipFileName, int maxFileSize, string filterName = null)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("filter:'{0}'", filterName);
            }

            if (!LongPathFile.Exists(zipFileName))
            {
                log.WarnFormat("File '{0}' does not exist.", zipFileName);
                yield break;
            }

            using (ZipInputStream s = new ZipInputStream(LongPathFile.Open(zipFileName, FileMode.Open, FileAccess.Read)))
            {
                log.InfoFormat("Processing zip file:'{0}'", zipFileName);
                ZipEntry theEntry;
                long c = 0;

                while ((theEntry = s.GetNextEntry()) != null)
                {
                    var name = theEntry.Name;

                    if (filterName != null && string.Compare(name, filterName, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        continue;
                    }

                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Zip:{0}\nName : {1}\nDate: {2}\nUncompressed: {3}\nCompressed: {4}", zipFileName, name, theEntry.DateTime, theEntry.Size, theEntry.CompressedSize);
                    }

                    if (theEntry.IsFile)
                    {
                        log.DebugFormat("The entry '{0}' in {1} is file.", name, zipFileName);
                        byte[] data = new byte[4096];
                        //TODO: check the performance of this thing
                        MemoryStream ms = new MemoryStream();
                        long totalSize = 0;
                        bool skip = false;
                        int size;

                        while ((size = s.Read(data, 0, data.Length)) > 0)
                        {
                            totalSize += size;

                            if (totalSize > maxFileSize)
                            {
                                log.InfoFormat("Skipping file '{0}'. It's bigger than {1}.", name, maxFileSize);
                                skip = true;
                                ms.Dispose();
                                break;
                            }

                            ms.Write(data, 0, size);
                        }

                        if (skip)
                        {
                            continue;
                        }
                        else
                        {
                            c++;
                            ms.Seek(0, SeekOrigin.Begin);
                            var fn = zipFileName + Path.AltDirectorySeparatorChar + name;
                            var t = new Tuple<string, Stream>(fn, ms);
                            yield return t;
                        }

                        if (filterName != null)
                        {
                            yield break;
                        }
                    }
                }

                log.InfoFormat("Got {0} files from '{1}'.", c, zipFileName);

                // Close can be ommitted as the using statement will do it automatically
                // but leaving it here reminds you that is should be done.
                //s.Close();
            }
        }
    }
}