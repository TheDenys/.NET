using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET4.MultiPatternSearch
{
    class MultiPatternSearch
    {
        /* We have to make a copy of resolved collection to avoid mixing resolved values for patterns that have common prefix:
         * a00000-b+bvvvvvv
         * 
         * p1 a*-b*
         * p2 a*+b*
         * 
         * 
         */
        public static void TraverseTree(NodesTree.Node node, string input, int pos, NodesTree.Node wcNode, int wcStart, Dictionary<string, string> resolvedWildcards, List<Tuple<NodesTree.Node, Dictionary<string, string>>> results)
        {
            var currentPattern = node.Value;
            var n = input.Length - 1;
            var isWildcard = node.IsWildcard;
            var success = false;
            var nextPos = -1;
            var resolved = false;

            if (isWildcard)
            {
                string resolvedWildcard;
                resolved = resolvedWildcards.TryGetValue(currentPattern, out resolvedWildcard);

                if (resolved)
                {
                    var lastPatternPos = GetPositionOfPatternLastCharacter(input, resolvedWildcard, pos);
                    success = nextPos != -1;
                    nextPos = lastPatternPos + 1;
                }
                else
                {
                    success = input[pos] != '/';
                    nextPos = pos + 1;
                }
            }
            else // string pattern
            {
                // root node
                if (currentPattern == "")
                {
                    success = true;
                    nextPos = 0;
                }
                else
                {
                    var lastPatternPos = GetPositionOfPatternLastCharacter(input, currentPattern, pos);
                    success = lastPatternPos != -1;
                    nextPos = lastPatternPos + 1;
                }
            }

            if (success)
            {
                if (nextPos <= n)
                {
                    foreach (var childNode in node.Nodes)
                    {
                        var rBuf = new Dictionary<string, string>(resolvedWildcards);

                        // previous node was an unresolved wildcard so resolve it now
                        if (wcNode != null)
                        {
                            var patternStartPos = nextPos - currentPattern.Length;
                            var resolvedLength = patternStartPos - wcStart;
                            if (resolvedLength < 1) throw new InvalidOperationException("Wildcard can't be matched to an empty string.");
                            var resolvedValue = input.Substring(wcStart, resolvedLength);
                            rBuf.Add(wcNode.Value, resolvedValue);
                        }

                        TraverseTree(childNode, input, nextPos, (isWildcard && !resolved) ? node : null, pos, rBuf, results);
                    }
                }

                if (node.CanTerminate)
                {
                    var rBuf = new Dictionary<string, string>(resolvedWildcards);

                    // current node is an unresolved wildcard, so resolve it
                    if (isWildcard && !resolved)
                    {
                        var resolvedValue = input.Substring(pos);
                        rBuf.Add(currentPattern, resolvedValue);
                    }

                    results.Add(Tuple.Create(node, rBuf));
                }
            }
        }

        public static int GetPositionOfPatternLastCharacter(string input, string pattern, int pos)
        {
            var match = false;
            var n = input.Length - 1;
            var m = pattern.Length - 1;
            var k = pos + m;

            while (k <= n)
            {
                // comparing the pattern from the end
                var ppos = m;

                for (int i = k; ; i--, ppos--)
                {
                    if (ppos < 0 || input[i] != pattern[ppos])
                    {
                        break;
                    }
                }

                if (ppos == -1)// walked through the whole pattern
                {
                    // k holds the position where pattern finishes
                    match = true;
                    break;
                }

                k++;
            }

            if (match)
            {
                return k;
            }

            // indicates absence of pattern any further than given pos in input
            return -1;
        }
    }
}
