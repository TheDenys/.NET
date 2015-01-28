using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class FileHelper
    {
        public static FileStream Create(String fName)
        {
            if (File.Exists(fName))
            {
                File.Delete(fName);
            }
            return File.Create(fName);
        }

        [Run(0)]
        public static void DoReplaceInFiles()
        {
            string fileNameWithFileNames = "files.txt";

            if (!File.Exists(fileNameWithFileNames))
            {
                ConsolePrint.print("can't find file {0}", fileNameWithFileNames);
                Environment.Exit(1);
            }

            var replacements = new List<KeyValuePair<string, string>>(3)
                                   {
                                       new KeyValuePair<string, string>(@"2010",@"2011"),
                                   };

            foreach (var fileName in File.ReadLines(fileNameWithFileNames))
            {
                var fullFileName = Path.GetFullPath(fileName);

                if (!File.Exists(fullFileName))
                {
                    ConsolePrint.print("can't find file {0}", fileName);
                    continue;
                }

                ConsolePrint.print("processing " + fullFileName);

                string content;
                string newContent = null;

                using (var reader = new StreamReader(fileName))
                {
                    content = reader.ReadToEnd();
                    newContent = content;
                }

                foreach (var keyValuePair in replacements)
                {
                    var searchText = keyValuePair.Key;
                    var replaceText = keyValuePair.Value;
                    newContent = newContent.Replace(searchText, replaceText);
                }

                if (newContent != content)
                {
                    ConsolePrint.print("modified");

                    using (var writer = new StreamWriter(fileName))
                    {
                        writer.Write(newContent);
                    }
                }
                else
                {
                    ConsolePrint.print("not modified");
                }
            }
        }

    }

    public class AppTest
    {
        public static void Execute()
        {
            String content = "hi guys рашин";
            byte[] buf = new UTF8Encoding(true).GetBytes(content);
            using(Stream fs = FileHelper.Create("1"))
            {
                fs.Write(buf, 0, buf.Length);
            }
        }
    }
}
