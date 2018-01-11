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
        public void BuildTree()
        {
            var root = MultiPatternSearch.BuildTree(new List<List<PatternElement>> {
                new List<PatternElement>{new StringPatternElement("a"),new WildcardPatternElement("x1"),new StringPatternElement("-b"),new WildcardPatternElement("x2")},
                new List<PatternElement>{new StringPatternElement("a"),new WildcardPatternElement("x1"),new StringPatternElement("+b"),new WildcardPatternElement("x2")},
                new List<PatternElement>{new StringPatternElement("a0"),new WildcardPatternElement("x1"),new StringPatternElement("-b"),new WildcardPatternElement("x2")},
                new List<PatternElement>{new StringPatternElement("a0"),new WildcardPatternElement("x1"),new StringPatternElement("+b"),new WildcardPatternElement("x2")},
                new List<PatternElement>{new WildcardPatternElement("x0"),new StringPatternElement("-b"),new WildcardPatternElement("x2")},
                new List<PatternElement>{new WildcardPatternElement("x0"),new StringPatternElement("+b"),new WildcardPatternElement("x2")},
            });

            var input = "a00-b+bvv";

            List<System.Tuple<NodesTree.Node, Dictionary<string, string>>> results = new List<System.Tuple<NodesTree.Node, Dictionary<string, string>>>();
            MultiPatternSearch.TraverseTreeAndCollectMatchingPatterns(root, input, 0, null, 0, new Dictionary<string, string>(), results);
        }

        [Run(1)]
        public void TreeGo()
        {
            /* We have to make a copy of resolved collection to avoid mixing resolved values for patterns that have common prefix:
             * a00-b+bvv
             * 
             * p1 a%x1%-b%x2%
             * p2 a%x1%+b%x2%
             * 
             * p3 a0%x1%-b%x2%
             * p4 a0%x1%+b%x2%
             * 
             * p5 %x0%-b%x2%
             * p6 %x0%+b%x2%
             * 
             */

            var root = NodesTree.BuildRootNode(
                NodesTree.BuildStringNode("a",
                    NodesTree.BuildWildCardNode("x1",
                        NodesTree.BuildStringNode("-b",
                            NodesTree.BuildWildCardNode("x2"))
                        ,
                        NodesTree.BuildStringNode("+b",
                            NodesTree.BuildWildCardNode("x2"))
                    )
                )
                ,
                NodesTree.BuildStringNode("a0",
                    NodesTree.BuildWildCardNode("x1",
                        NodesTree.BuildStringNode("-b",
                            NodesTree.BuildWildCardNode("x2"))
                        ,
                        NodesTree.BuildStringNode("+b",
                            NodesTree.BuildWildCardNode("x2"))
                    )
                )
                ,
                NodesTree.BuildWildCardNode("x0",
                        NodesTree.BuildStringNode("-b",
                            NodesTree.BuildWildCardNode("x2"))
                        ,
                        NodesTree.BuildStringNode("+b",
                            NodesTree.BuildWildCardNode("x2"))
                )
                );

            //           012345678
            var input = "a00-b+bvv";

            List<System.Tuple<NodesTree.Node, Dictionary<string, string>>> results = new List<System.Tuple<NodesTree.Node, Dictionary<string, string>>>();
            MultiPatternSearch.TraverseTreeAndCollectMatchingPatterns(root, input, 0, null, 0, new Dictionary<string, string>(), results);
        }
    }
}
