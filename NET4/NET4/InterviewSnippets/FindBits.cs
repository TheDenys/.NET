using System;
using System.Collections;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.InterviewSnippets
{
    [RunableClass]
    public class FindBits
    {

        [Run(0)]
        protected void Find()
        {
            int k = 4;

            int[] values = new int[] { 0, 1, 1, 0, 1, 0, 1, 0, 0, 1, 1, 1, 0, 1, 0 };
            int N = values.Length;
            int[] positions = new int[N];

            int c = 0;
            int minLength = N + 1;

            for (int i = 0; i < N; i++)
            {
                if (values[i] == 1)
                {
                    positions[c] = i;

                    if (c >= k - 1)
                    {
                        int l = positions[c] - positions[c - (k - 1)] + 1;

                        if (l < minLength)
                        {
                            minLength = l;
                        }
                    }

                    c++;
                }
            }

            if (minLength == N + 1)
            {
                minLength = 0;
            }

            Console.WriteLine("minLength={0}", minLength);
        }

    }
}