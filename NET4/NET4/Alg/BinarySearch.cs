using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Alg
{
    [RunableClass]
    public class BinarySearch : RunableBase
    {
        [Run(0)]
        protected void Search()
        {
            double[] arr = new[] { 0.01, 0.02, 0.04, 0.05, 0.07, 0.08, 1.1, 1.3 };
            double[] boundaries = new[] { 0.001, 0.03, 0.07, 1.1, 2.0 };
            double d = 0.03;
            int last = 0;

            foreach (var b in boundaries)
            {
                int p1 = last < 0 ? 0 : last;
                int p2 = arr.Length - 1;

                while (p2 - p1 > 1)
                {
                    if (b < arr[p1])
                    {
                        p2 = -1;
                        break;
                    }

                    if (arr[p2] <= b)
                    {
                        p2 = p1 = arr.Length;
                        break;
                    }

                    int mid = (p2 + p1) / 2;
                    if ((p2 + p1 + 1) % 2 == 0) mid++;
                    if (b == arr[mid])
                    {
                        p2 = p1 = mid;
                    }
                    else if (b < arr[mid])
                    {
                        p2 = mid;
                    }
                    else
                    {
                        p1 = mid;
                    }

                    DebugFormat("p1: {0}, p2: {1}, mid: {2} a({0})={3}, a({1})={4}", p1, p2, mid, arr[p1], arr[p2]);
                }

                DebugFormat("Boundary pos: {0}, count: {1}", p2, p2 - (last == 0 ? -1 : last));
                last = p2;
            }
        }
    }
}