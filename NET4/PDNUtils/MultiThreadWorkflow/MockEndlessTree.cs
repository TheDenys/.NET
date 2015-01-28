using System.Collections.Generic;
using System.Threading;

namespace PDNUtils.MultiThreadWorkflow
{
    public class MockEndlessTree
    {
        private int childrenCount = 10;

        public IEnumerable<NodeBase> GetChildNodes(NodeBase node)
        {
            if (node.Path.Length > 9)
            {
                yield break;
            }

            if (node is Node)
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    Thread.Sleep(20);
                    yield return new Node { Path = node.Path + "/n" + i };
                }

                for (int i = 0; i < childrenCount; i++)
                {
                    Thread.Sleep(20);
                    yield return new Leaf { Path = node.Path + "/l" + i };
                }
            }
        }
    }

    public abstract class NodeBase
    {
        public string Path;
    }

    public class Node : NodeBase
    {

    }

    public class Leaf : NodeBase
    {
    }
}