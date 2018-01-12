using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET4.MultiPatternSearch
{
    class MultiPatternSearch
    {
        /// <summary>
        /// Builds a tree from given collection of patterns. Patterns with common beginning are attached to the same node.
        /// Patterns can contain arbitrary amount of wildcards. Wildcards are identified by name and can be used as constraints. E.g. pattern %x0%-%x0%, where %x0% is a wildcard, will match "a-a" but not "a-b".
        /// Example:
        ///  patterns: "abc" "abd" "ab*" will produce tree ('' ('ab' ('c' 'd' '*')))
        /// The resulting tree can be used in <see cref="FindPatternsAndResolveWildcards(NodesTree.Node, string)"/>.
        /// </summary>
        /// <param name="collectionOfPatterns">Patterns</param>
        /// <returns>Root node of the tree</returns>
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
                if (elementPosition < patternElements.Count)
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
            FindNodeWithSameStartingPatternElementsRecursive(startNode, patternElements, -1, ref f, ref nodeAndPosition);
            return nodeAndPosition;
        }

        private static void FindNodeWithSameStartingPatternElementsRecursive(NodesTree.Node node, List<PatternElement> patternElements, int elementPosition, ref bool found, ref Tuple<NodesTree.Node, int> result)
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
                        FindNodeWithSameStartingPatternElementsRecursive(child, patternElements, nextElementPosition, ref found, ref result);
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

        /// <summary>
        /// Looks for terminating nodes that would correspond to patterns matching given input.
        /// Example:
        ///  patterns: "abc" "abd" "ab*" will -> tree ('' ('ab' ('c' 'd' '*')))
        ///  input "abc" would return 2 results:
        ///   for pattern "abc" with no wildcards
        ///   for pattern "ab*" the wildcard value will be "c"
        /// </summary>
        /// <param name="root"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<System.Tuple<NodesTree.Node, Dictionary<string, string>>> FindPatternsAndResolveWildcards(NodesTree.Node root, string input)
        {
            List<System.Tuple<NodesTree.Node, Dictionary<string, string>>> results = new List<System.Tuple<NodesTree.Node, Dictionary<string, string>>>();
            MultiPatternSearch.TraverseTreeAndCollectMatchingPatternsRecursive(root, input, 0, null, 0, new Dictionary<string, string>(), results);
            return results;
        }

        /* We have to make a copy of resolved collection to avoid mixing resolved values for patterns that have common prefix:
         * Example: 
         * input: a00-b+bvv
         * p1: a*-b*
         * p2: a*+b*
         */
        private static void TraverseTreeAndCollectMatchingPatternsRecursive(NodesTree.Node node, string input, int pos, NodesTree.Node wcNode, int wcStart, Dictionary<string, string> resolvedWildcards, List<Tuple<NodesTree.Node, Dictionary<string, string>>> results)
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
                    success = lastPatternPos != -1;
                    nextPos = lastPatternPos + 1;
                }
                else
                {
                    // we want wildcard be a non-empty and non-slash, change this according to specific wildcard requirements
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

                        TraverseTreeAndCollectMatchingPatternsRecursive(childNode, input, nextPos, (isWildcard && !resolved) ? node : null, pos, rBuf, results);
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

        /// <summary>
        /// Finds a position of the first occurrence of given pattern in the input starting at given position.
        ///  input: "aaaacmelab" pattern: "acme" position: 0 result: 6
        ///  input: "aaaacmelab" pattern: "acme" position: 3 result: 6
        ///  input: "aaaacmelab" pattern: "acme" position: 4 result: -1
        /// </summary>
        /// <param name="input">String that may contain a pattern</param>
        /// <param name="pattern">Pattern to look for</param>
        /// <param name="position">Start position where to look for pattern in the input.</param>
        /// <returns>Position of the pattern last character if it was found, otherwise -1</returns>
        public static int GetPositionOfPatternLastCharacter(string input, string pattern, int position)
        {
            var match = false;
            var n = input.Length - 1;
            var m = pattern.Length - 1;
            var k = position + m;

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
