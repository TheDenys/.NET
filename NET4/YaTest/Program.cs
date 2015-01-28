using System;
using System.IO;

namespace YaTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Process();
            }
            // it's a "pokemon" catch condition but it must be sufficient for this app
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadLine();
        }

        // sample input data
        static string input =
@"3
abacaba
mycabarchive
acabistrue";

        // parses input data and calls subroutine for longest substring search
        static void Process()
        {
            var sr = new StringReader(input);
            var linesStr = sr.ReadLine();
            int k;

            // check if we can parse K
            if (!int.TryParse(linesStr, out k))
            {
                throw new ApplicationException("Can't parse number K in the very first line of input data.");
            }
            
            // check if K belongs to allowed interval
            var validSpan = 1 <= k && k <= 10;
            
            if (!validSpan)
            {
                throw  new ArgumentOutOfRangeException("K must be in [1..10] interval.");
            }

            // reading lines
            var strings = new string[k];

            for (int i = 0; i < k; i++)
            {
                strings[i] = sr.ReadLine();
                
                // line should be maximum 10000 characters long
                if (strings[i].Length > 10000)
                {
                    throw new ApplicationException("Maximum string length is exceeded. Maximum is 10000.");
                }
            }

            FindLongestCommon(k, strings);
        }

        // looks for longest common substring among k strings given as parameter
        static void FindLongestCommon(int k, string[] strings)
        {
            // assume there is no common string in the beginning
            string longest = string.Empty;
            bool skip = false;

            // we give finish condition is such a way that we'll stop search if there are not enough characters left
            // e.g. we have found greatest common "foo" and first line is "foobar"
            // we won't try to find "bar" in givent strings, as it is the same long as "foo"
            for (int pos = 0; pos < strings[0].Length - longest.Length; pos++)
            {
                skip = false;

                // we try to find sample longer than previous one thus setting length = longest.Length + 1
                for (int length = longest.Length + 1; length <= strings[0].Length - pos; length++)
                {
                    // we make sample by getting more and more characters from the first string:
                    // e.g.: "b", "ba", "bar"
                    var sample = strings[0].Substring(pos, length);

                    // check if sample is present in all other strings as well
                    for (int stringIdx = 1; stringIdx < k; stringIdx++)
                    {
                        var sampleIdx = strings[stringIdx].IndexOf(sample);
                        
                        // -1 means absence of given substring in a string and we need to check next combination
                        if (sampleIdx == -1)
                        {
                            skip = true;
                            break;
                        }
                    }

                    // this condition allows us to skip redundant checks
                    // for instance we haven't found "ba" so it's no worth to look for "bar"
                    if (skip)
                    {
                        break;
                    }
                    else
                    {
                        // assume current sample is a longest common substring
                        // and try to find longer one
                        longest = sample;
                    }
                }
            }

            // output longest common string
            Console.WriteLine(longest);
        }
    
    }
}
