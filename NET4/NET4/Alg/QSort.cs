using System;
using System.Collections.Generic;
using System.Threading;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Alg
{
    [RunableClass]
    internal class QSort : RunableBase
    {
        [Run(0)]
        protected void Go()
        {
            //int[] a = { 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            int[] a = { 9, 2, 7, 6, 5, 7, 8, 5, 1 };
            Sort(a, 0, a.Length - 1);
            Debug(a);
        }

        // so wrong
        static void Sort(int[] a, int M, int N)
        {
            int i = M, j = N, p;

            p = a[N >> 1];

            do
            {
                while (a[i] < p) i++;
                while (a[j] > p) j--;

                if (i <= j)
                {
                    a[j] = Interlocked.Exchange(ref a[i], a[j]);
                    i++;
                    j--;
                }

            } while (i <= j);

            if (j > 0) Sort(a, M, j);
            if (N > i) Sort(a, i, N - i);
        }


        //private static void Sort<T>(T[] source) where T : IComparable
        //{
        //    int i = 0;
        //    int j = source.Length;
        //    sbyte s = -1;

        //    do
        //    {

        //    } while (i != j);
        //}

        //private static void Exchange<T>(T[] arr, int i, int j)
        //{
        //    T tmp = arr[i];
        //    arr[i] = arr[j];
        //    arr[j] = tmp;
        //}
    }
}