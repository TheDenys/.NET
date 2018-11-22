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

        [Run(0)]
        public void BuildTree()
        {
            var root = MultiPatternSearch.BuildTree(new List<List<PatternElement>> {
                new List<PatternElement>{new StringPatternElement("a"),new WildcardPatternElement("x1"),new StringPatternElement("-b"),new WildcardPatternElement("x2")},
                new List<PatternElement>{new StringPatternElement("a"),new WildcardPatternElement("x1"),new StringPatternElement("+b"),new WildcardPatternElement("x2")},
                new List<PatternElement>{new StringPatternElement("a0"),new WildcardPatternElement("x1"),new StringPatternElement("-b"),new WildcardPatternElement("x2")},
                new List<PatternElement>{new StringPatternElement("a0"),new WildcardPatternElement("x1"),new StringPatternElement("+b"),new WildcardPatternElement("x2")},
                new List<PatternElement>{new WildcardPatternElement("x0"),new StringPatternElement("-b"),new WildcardPatternElement("x2")},
                new List<PatternElement>{new WildcardPatternElement("x0"),new StringPatternElement("+b"),new WildcardPatternElement("x2")},
                new List<PatternElement>{new WildcardPatternElement("x0"),new StringPatternElement("-"),new WildcardPatternElement("x0")},
                new List<PatternElement>{new StringPatternElement("a"),new WildcardPatternElement("x0"),new StringPatternElement("vv")},
            });

            var input = "a00-b+bvv";

            List<System.Tuple<NodesTree.Node, Dictionary<string, string>>> results = MultiPatternSearch.FindPatternsAndResolveWildcards(root, input);

            var root2 = MultiPatternSearch.BuildTree(new List<List<PatternElement>> {
                new List<PatternElement>{new WildcardPatternElement("x0"),new StringPatternElement("-"),new WildcardPatternElement("x0")},
            });

            var input2 = "a-a";
            var results2 = MultiPatternSearch.FindPatternsAndResolveWildcards(root2, input2);// should produce 1 result

            var input3 = "a-b";
            var results3 = MultiPatternSearch.FindPatternsAndResolveWildcards(root2, input3);// should produce 0 results

            var root3 = MultiPatternSearch.BuildTree(new List<List<PatternElement>> {
                new List<PatternElement>{new WildcardPatternElement("x0"),new StringPatternElement("-"),new WildcardPatternElement("x1")},
            });

            var input4 = "a-a";
            var results4 = MultiPatternSearch.FindPatternsAndResolveWildcards(root3, input4);// should produce 1 result

            var input5 = "a-b";
            var results5 = MultiPatternSearch.FindPatternsAndResolveWildcards(root3, input5);// should produce 0 results
        }

        [Run(0)]
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
                //NodesTree.BuildStringNode("a",
                //    NodesTree.BuildWildCardNode("x1",
                //        NodesTree.BuildStringNode("-b",
                //            NodesTree.BuildWildCardNode("x2"))
                //        ,
                //        NodesTree.BuildStringNode("+b",
                //            NodesTree.BuildWildCardNode("x2"))
                //    )
                //)
                //,
                //NodesTree.BuildStringNode("a0",
                //    NodesTree.BuildWildCardNode("x1",
                //        NodesTree.BuildStringNode("-b",
                //            NodesTree.BuildWildCardNode("x2"))
                //        ,
                //        NodesTree.BuildStringNode("+b",
                //            NodesTree.BuildWildCardNode("x2"))
                //    )
                //)
                //,
                //NodesTree.BuildWildCardNode("x0",
                //        NodesTree.BuildStringNode("-b",
                //            NodesTree.BuildWildCardNode("x2"))
                //        ,
                //        NodesTree.BuildStringNode("+b",
                //            NodesTree.BuildWildCardNode("x2"))
                //)
                //,
                NodesTree.BuildStringNode("a",
                        NodesTree.BuildWildCardNode("x0",
                            NodesTree.BuildStringNode("/vv"))
                )
                ,
                NodesTree.BuildStringNode("a",
                        NodesTree.BuildWildCardNode("x0",
                            NodesTree.BuildStringNode("/vv_2"))
                )
                );

            //           012345678901
            var input = "a00-b+b/vv_2";

            List<System.Tuple<NodesTree.Node, Dictionary<string, string>>> results = MultiPatternSearch.FindPatternsAndResolveWildcards(root, input);
        }
    }
}
