using System;
using System.IO;
using System.Linq;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P022_NamesScores : RunableBase
    {
        [Run(0)]
        public void Solve()
        {
            var nameScoreTotal = File.ReadAllLines(@"Euler\p022_names.txt")
                .SelectMany(s => s.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(s => s.Substring(1, s.Length - 2))
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                .Select((s, pos) => (pos + 1) * s.Aggregate(0, (chSum, ch) => chSum + ch - 64))
                .Sum();
            DebugFormat("Total: {0}", nameScoreTotal);
        }
    }
}