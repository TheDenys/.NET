using System.Collections.Generic;
using PDNUtils.Runner.Attributes;

namespace NET4.MultiPatternSearch
{
    [RunableClass]
    class MultiSearchTest
    {
        [Run(0)]
        public void Go()
        {
            var patternLastPos = MultiPatternSearch.GetPositionOfPatternLastCharacter("aaaacmelab", "acme", 0);// expect 6
            patternLastPos = MultiPatternSearch.GetPositionOfPatternLastCharacter("aaaacmelab", "acme", 3);// expect 6
            patternLastPos = MultiPatternSearch.GetPositionOfPatternLastCharacter("aaaacmelab", "acme", 4);// expect -1
        }

        [Run(1)]
        public void TreeGo()
        {
            /* We have to make a copy of resolved collection to avoid mixing resolved values for patterns that have common prefix:
             * a00000-b+bvvvvvv
             * 
             * p1 a*-b*
             * p2 a*+b*
             * 
             * 
             */

            var root = NodesTree.BuildRootNode(
                NodesTree.BuildStringNode("a",
                    NodesTree.BuildWildCardNode("x1",
                        NodesTree.BuildStringNode("-b",
                            NodesTree.BuildWildCardNode("x2")),
                        NodesTree.BuildStringNode("+b",
                            NodesTree.BuildWildCardNode("x2"))
                    )
                )
                //, NodesTree.BuildStringNode("ab")
                //, NodesTree.BuildStringNode("cd")
                );

            var input = "a00-b+bvv";

            List<System.Tuple<NodesTree.Node, Dictionary<string, string>>> results = new List<System.Tuple<NodesTree.Node, Dictionary<string, string>>>();
            MultiPatternSearch.TraverseTree(root, input, 0, null, 0, new Dictionary<string, string>(), results);
        }
    }
}
