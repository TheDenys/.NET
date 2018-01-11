using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET4.MultiPatternSearch
{
    class MultiPatternSearch
    {
        public static NodesTree.Node BuildTree(List<List<PatternElement>> collectionOfPatterns)
        {
            var root = NodesTree.BuildRootNode();

            foreach (var patternElements in collectionOfPatterns)
            {
                // find node with the same starting pattern parts
                var nodeAndElementPosition = FindNodeWithSameStartingPatternElements(root, patternElements);
                var node = nodeAndElementPosition.Item1;
                // increase element position to the next one
                var elementPosition = nodeAndElementPosition.Item2 + 1;

                // attach remaining pattern parts to the node, if there are any
                if (elementPosition < patternElements.Count - 1)
                {
                    var lastAddedNode = node;

                    for (var pos = elementPosition; pos < patternElements.Count; pos++)
                    {
                        PatternElement currentElement = patternElements[pos];
                        NodesTree.Node nodeToAdd = currentElement is WildcardPatternElement ? NodesTree.BuildWildCardNode(currentElement.Value) : NodesTree.BuildStringNode(currentElement.Value);
                        lastAddedNode.Nodes.Add(nodeToAdd);
                        lastAddedNode = nodeToAdd;
                    }
                }
            }

            return root;
        }

        private static Tuple<NodesTree.Node, int> FindNodeWithSameStartingPatternElements(NodesTree.Node startNode, List<PatternElement> patternElements)
        {
            Tuple<NodesTree.Node, int> nodeAndPosition = Tuple.Create(startNode, -1);
            var f = false;
            FindNodeWithSameStartingPatternElements(startNode, patternElements, -1, ref f, ref nodeAndPosition);
            return nodeAndPosition;
        }

        private static void FindNodeWithSameStartingPatternElements(NodesTree.Node node, List<PatternElement> patternElements, int elementPosition, ref bool found, ref Tuple<NodesTree.Node, int> result)
        {
            if (found) return;
            var nodeValue = node.Value;
            var isRoot = nodeValue == "";
            var valuesCorrespond = isRoot;
            var nextElementPosition = elementPosition + 1;

            if (!isRoot)
            {
                var currentElement = patternElements[elementPosition];
                bool isWcNode = node.IsWildcard;
                bool isWcPattern = currentElement is WildcardPatternElement;
                var typesCorrespond = !(isWcNode ^ isWcPattern);// negated XOR, i.e. logical biconditional returns true only if both operands are the same
                valuesCorrespond = typesCorrespond && nodeValue == currentElement.Value;
            }

            if (valuesCorrespond)
            {
                // check if this is the last position in pattern
                // yes: set finish to true and mark node as able to terminate
                if (elementPosition == patternElements.Count - 1)
                {
                    found = true;
                    node.CanTerminate = true;
                    result = Tuple.Create(node, elementPosition);
                }
                // no: check child nodes with further elements of the pattern
                else
                {
                    foreach (var child in node.Nodes)
                    {
                        if (found) break;
                        FindNodeWithSameStartingPatternElements(child, patternElements, nextElementPosition, ref found, ref result);
                    }

                    // no further pattern parts have been found so this one is the last common part and we should return this node and position
                    if (!found)
                    {
                        found = true;
                        result = Tuple.Create(node, elementPosition);
                    }
                }
            }
        }

        private static bool NodeTypeCorrespondsPatternElement(NodesTree.Node node, PatternElement patternElement)
        {
            if (patternElement is WildcardPatternElement)
            {
                return node.IsWildcard;
            }
            else
            {
                return !node.IsWildcard;
            }
        }

        /* We have to make a copy of resolved collection to avoid mixing resolved values for patterns that have common prefix:
         * Example: 
         * input: a00-b+bvv
         * p1: a*-b*
         * p2: a*+b*
         */
        public static void TraverseTreeAndCollectMatchingPatterns(NodesTree.Node node, string input, int pos, NodesTree.Node wcNode, int wcStart, Dictionary<string, string> resolvedWildcards, List<Tuple<NodesTree.Node, Dictionary<string, string>>> results)
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

                        TraverseTreeAndCollectMatchingPatterns(childNode, input, nextPos, (isWildcard && !resolved) ? node : null, pos, rBuf, results);
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
