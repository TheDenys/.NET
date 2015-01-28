using System;
using System.IO;

namespace PDNUtils.IO
{
    public class PathUtils
    {
        /// <summary>
        /// Rolls file names in the given folder with increasing numeric extension (like log4net does).
        /// When there are files
        ///  a.log
        ///  a.log.1
        ///  a.log.2
        /// after this method call files will become:
        ///  a.log.1
        ///  a.log.2
        ///  a.log.3
        /// If fileName doesn't exist on disk this method will do nothing.
        /// Files that exceed limit of maxFiles will be deleted.
        /// </summary>
        /// <param name="directory">Folder with files to be rolled out.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="maxFiles">maximum amount of files</param>
        public static void RollFilenames(string directory, string fileName, int maxFiles)
        {
            if (!File.Exists(Path.Combine(directory, fileName)))
            {
                return;
            }

            var files = GetFilesSorted(directory, fileName);

            if (files.Length == 0)
            {
                return;
            }

            for (int i = files.Length - 1; i >= 1; i--)
            {
                if (i + 1 >= maxFiles)
                {
                    File.Delete(files[i]);
                }
                else
                {
                    File.Move(files[i], Path.ChangeExtension(files[i], (i + 1).ToString()));
                }
            }

            File.Move(files[0], files[0] + ".1");
        }

        private static string[] GetFilesSorted(string directory, string fileName)
        {
            var files = Directory.GetFiles(directory, fileName + ".*");
            CompareNumericExtensions(fileName, files);
            return files;
        }

        public static void CompareNumericExtensions(string fileName, string[] files)
        {
            Array.Sort(files,
                (string s1, string s2) =>
                {
                    int e1 = GetNumericExtension(fileName, s1);
                    int e2 = GetNumericExtension(fileName, s2);
                    return e1 - e2;
                });
        }

        /// <summary>
        /// Parses file names and returns numeric result representing extesion.
        /// If filnae pattern and file name don't match -1 returns.
        /// Samples:
        /// fileNamePattern | fileName         | Result 
        /// foo.bar         | folder\foo.bar   |   0
        /// foo.bar         | folder\foo.bar.1 |   1
        /// foo.bar         | folder\foo.bar.2 |   2
        /// foo.bar         | folder\foo.bar.3 |   3
        /// bar.foo         | folder\foo.bar   |  -1
        /// </summary>
        /// <param name="fileNamePattern">File name pattern. I.e. filename including extension.</param>
        /// <param name="fileName">Filename used to fetch extension.</param>
        /// <returns>0 when filename exactly matches filename, number represnting extension when file name mathes pattern, -1 otherwise</returns>
        private static int GetNumericExtension(string fileNamePattern, string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            var fn = Path.GetFileName(fileName);

            if (string.Compare(fileNamePattern, fn, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return 0;
            }

            if (!fn.StartsWith(fileNamePattern, StringComparison.InvariantCultureIgnoreCase))
            {
                return -1;
            }

            int i = fn.LastIndexOf(".", fn.Length, StringComparison.InvariantCultureIgnoreCase);
            string ext = i < 0 ? string.Empty : fn.Substring(i + 1);

            if (ext == string.Empty)
            {
                return 0;
            }

            int e;

            if (int.TryParse(ext, out e))
            {
                return e;
            }

            return -1;
        }

    }
}