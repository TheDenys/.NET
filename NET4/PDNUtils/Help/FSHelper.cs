using System;
using System.IO;

namespace PDNUtils.Help
{
    /// <summary>
    /// Class with methods for work with files
    /// </summary>
    public static class FSHelper
    {

        /// <summary>
        /// Generates new file name which doesn't exists in folder with the given file
        /// For file name "foobar.txt" will return "foobar(1).txt" if "foobar.txt" already exists.
        /// Otherwise returns "foobar.txt"
        /// </summary>
        /// <param name="name">full file name</param>
        /// <returns>the name of file which doesn't exist yet</returns>
        public static string GetNextFileName(string name)
        {
            int count = 1;
            string folder = name != null ? Path.GetDirectoryName(name) : "";
            string f_name_pref = name != null ? Path.GetFileNameWithoutExtension(name) : "debug";
            string f_name_ext = name != null ? Path.GetExtension(name) : "";
            string f_name_res = f_name_pref;
            while (File.Exists(f_name_res = Path.Combine(folder, f_name_res + f_name_ext)))
            {
                f_name_res = string.Format("{0}({1})", f_name_pref, count++);
            }
            return f_name_res;
        }

        /// <summary>
        /// Creates utf8 encoded file with given content
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <param name="content">content to be writtten</param>
        public static void SaveToFileUTF8(string fileName, string content)
        {
            var fs = new FileStream(fileName, FileMode.Create);
            var sr = new StreamWriter(fs, System.Text.Encoding.UTF8);
            sr.Write(content);
            sr.Close();
        }

        /// <summary>
        /// replaces '/' with '\' and removes redundant '\'
        /// i.e. for path c:\\a/b will return c:\a\b
        /// </summary>
        /// <param name="path">path</param>
        /// <returns>fixed path</returns>
        public static string FixBackSlashes(string path)
        {
            path = path.Replace(@"/", @"\");
            path = path.Replace(@"\\", @"\");
            return path;
        }

        /// <summary>
        /// Combine base folder and path in an appropriate way
        /// removes redundant slashes, fix backslahes, and so on
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CombineAndFixPath(string basePath, string path)
        {
            path = FSHelper.FixBackSlashes(path);
            string res = Path.Combine(basePath, path);
            ConsolePrint.printMap(path, res);
            return res;
        }

        /// <summary>
        /// Fetch the directory name from <paramref name="pathWithSubFolders"/> according to <paramref name="basePath"/>
        /// example: <paramref name="basePath"/> is "c:\foo" and <paramref name="pathWithSubFolders"/> is "c:\foo\bar"
        /// result is "bar"
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="pathWithSubFolders"></param>
        /// <returns></returns>
        public static string FetchFolderName(string basePath, string pathWithSubFolders)
        {
            basePath = FSHelper.FixBackSlashes(basePath).ToLowerInvariant();
            pathWithSubFolders = FSHelper.FixBackSlashes(pathWithSubFolders).ToLowerInvariant();
            if (!pathWithSubFolders.StartsWith(basePath) || string.Compare(pathWithSubFolders, basePath, true) == 0)
                return null;
            var startPathArr = basePath.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            var cacheFolderArr = pathWithSubFolders.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            var res = cacheFolderArr[startPathArr.Length];
            res = Path.Combine(basePath, res);
            return res;
        }


    }
}
