using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class ZipTest
    {
        private string zippath = @"c:\PDN\PROJECTS\NET4\тестзип.zip";

        [Run(0)]
        protected void Go()
        {
            var files = GetFiles(zippath);

            foreach (var tuple in files)
            {
                ConsolePrint.print("fn:{0}", tuple.Item1);
                using (var sr = new StreamReader(tuple.Item2))
                {
                    ConsolePrint.print(sr.ReadToEnd());
                }
            }
            //using (ZipInputStream s = new ZipInputStream(File.OpenRead(zippath)))
            //{

            //    ZipEntry theEntry;
            //    while ((theEntry = s.GetNextEntry()) != null)
            //    {
            //        ConsolePrint.print("Name : {0}", theEntry.Name);
            //        ConsolePrint.print("Date : {0}", theEntry.DateTime);
            //        ConsolePrint.print("Size : (-1, if the size information is in the footer)");
            //        ConsolePrint.print("      Uncompressed : {0}", theEntry.Size);
            //        ConsolePrint.print("      Compressed   : {0}", theEntry.CompressedSize);

            //        if (theEntry.IsFile)
            //        {

            //            // Assuming the contents are text may be ok depending on what you are doing
            //            // here its fine as its shows how data can be read from a Zip archive.
            //            ConsolePrint.print("Show entry text (y/n) ?");

            //            if (Console.ReadLine() == "y")
            //            {
            //                byte[] data = new byte[4096];
            //                //TODO: check the performance of this thing
            //                MemoryStream ms = new MemoryStream();
            //                int size;
            //                while ((size = s.Read(data, 0, data.Length)) > 0)
            //                {
            //                    ms.Write(data, 0, size);
            //                    //ConsolePrint.print(Encoding.ASCII.GetString(data, 0, size));
            //                }

            //                ms.Seek(0, SeekOrigin.Begin);

            //                ConsolePrint.print(new StreamReader(ms).ReadToEnd());
            //            }
            //            ConsolePrint.print();
            //        }
            //    }

            //    // Close can be ommitted as the using statement will do it automatically
            //    // but leaving it here reminds you that is should be done.
            //    s.Close();
            //}

        }

        protected IEnumerable<Tuple<string, Stream>> GetFiles(string fileName)
        {
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zippath)))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    ConsolePrint.print("Name : {0}", theEntry.Name);
                    ConsolePrint.print("Date : {0}", theEntry.DateTime);
                    ConsolePrint.print("Size : (-1, if the size information is in the footer)");
                    ConsolePrint.print("      Uncompressed : {0}", theEntry.Size);
                    ConsolePrint.print("      Compressed   : {0}", theEntry.CompressedSize);

                    if (theEntry.IsFile)
                    {

                        // Assuming the contents are text may be ok depending on what you are doing
                        // here its fine as its shows how data can be read from a Zip archive.
                        //ConsolePrint.print("Show entry text (y/n) ?");

                        //if (Console.ReadLine() == "y")
                        {
                            byte[] data = new byte[4096];
                            //TODO: check the performance of this thing
                            MemoryStream ms = new MemoryStream();
                            int size;
                            while ((size = s.Read(data, 0, data.Length)) > 0)
                            {
                                ms.Write(data, 0, size);
                                //ConsolePrint.print(Encoding.ASCII.GetString(data, 0, size));
                            }

                            ms.Seek(0, SeekOrigin.Begin);

                            var fn = fileName + Path.DirectorySeparatorChar + theEntry.Name.Replace('/', Path.DirectorySeparatorChar);
                            var t = new Tuple<string, Stream>(fn, ms);
                            yield return t;
                            //ConsolePrint.print(new StreamReader(ms).ReadToEnd());
                        }
                        //ConsolePrint.print();
                    }
                }

                // Close can be ommitted as the using statement will do it automatically
                // but leaving it here reminds you that is should be done.
                //s.Close();
            }
        }
    }
}