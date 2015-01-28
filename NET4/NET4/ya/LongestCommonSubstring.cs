using System;
using System.IO;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.ya
{
    [RunableClass]
    public class LongestCommonSubstring : RunableBase
    {
//        private string input =
//@"3
//abacaba
//mycabarchive
//acabistrue";
        private string input =
@"3
abacabacacaca
mycabarchivecacaca
acabistruecacaca";

        public LongestCommonSubstring(MessageHandler mh)
            : base(mh)
        {
        }

        [Run(0)]
        protected void Parse()
        {
            var sr = new StringReader(input);
            var linesStr = sr.ReadLine();
            var k = Convert.ToInt32(linesStr);
            var strings = new string[k];

            for (int i = 0; i < k; i++)
            {
                strings[i] = sr.ReadLine();
            }

            Find(k, strings);
        }

        protected void Find(int k, string[] strings)
        {
            string longest = string.Empty;
            bool skip = false;

            for (int iFirstStart = 0; iFirstStart < strings[0].Length - longest.Length; iFirstStart++)
            {
                skip = false;

                for (int length = longest.Length + 1; length <= strings[0].Length - iFirstStart; length++)
                {
                    var sample = strings[0].Substring(iFirstStart, length);

                    for (int stringIdx = 1; stringIdx < k; stringIdx++)
                    {
                        var sampleIdx = strings[stringIdx].IndexOf(sample);
                        if (sampleIdx == -1)
                        {
                            skip = true;
                            break;
                        }
                    }

                    if (skip)
                    {
                        break;
                    }
                    else
                    {
                        longest = sample;
                    }
                }
            }

            MessageHandler.Info("longest:" + longest);
        }
    }
}