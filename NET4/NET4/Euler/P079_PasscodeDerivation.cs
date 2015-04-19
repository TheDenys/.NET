using System.Collections.Generic;
using System.IO;
using System.Linq;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P079_PasscodeDerivation : RunableBase
    {
        [Run(0)]
        protected void SolveIt()
        {
            List<IEnumerable<int>> attempts = File.ReadAllLines(@"Euler\p079_keylog.txt").Select(s => s.ToCharArray().Select(c => int.Parse(c.ToString()))).ToList();
            List<int> uniq = attempts.SelectMany(i => i.Select(n => n)).Distinct().OrderBy(i => i).ToList();
            Graph graph = new Graph();

            foreach (var attempt in attempts)
            {
                foreach (var connection in GetConnections(attempt))
                {
                    graph[connection.Key].Add(connection.Value);
                }
            }

            foreach (var g in graph)
            {
                DebugFormat("{0} : {1}", g.Key, string.Join(" ", g.Value));
            }

            int limit = 1;
            int token = -1;
            List<int> passItems = new List<int>(uniq.Count);

            while (limit < uniq.Count)
            {
                var buf = graph.First(kv => kv.Value.Count <= limit && (token != -1 ? kv.Value.Contains(token) : true));

                if (token == -1)
                {
                    var last = buf.Value.First();
                    passItems.Add(last);
                    DebugFormat("Found last code item: {0}", last);
                }

                limit++;
                token = buf.Key;
                passItems.Add(token);

                DebugFormat("Found next item: {0}", token);
            }

            passItems.Reverse();
            DebugFormat("Passcode: {0}", string.Join("", passItems));
        }

        IEnumerable<KeyValuePair<int, int>> GetConnections(IEnumerable<int> attempt)
        {
            var attemptItems = attempt.ToList();

            for (var i = 0; i <= attemptItems.Count - 2; i++)
            {
                for (var j = i + 1; j < attemptItems.Count; j++)
                {
                    yield return new KeyValuePair<int, int>(attemptItems[i], attemptItems[j]);
                }
            }
        }
    }

    public class Graph : Dictionary<int, ISet<int>>
    {
        public new ISet<int> this[int i]
        {
            get
            {
                ISet<int> set;
                if (TryGetValue(i, out set))
                {
                    return set;
                }
                else
                {
                    var sortedSet = new SortedSet<int>();
                    Add(i, sortedSet);
                    return sortedSet;
                }
            }

            set
            {
                base[i] = value;
            }
        }
    }
}