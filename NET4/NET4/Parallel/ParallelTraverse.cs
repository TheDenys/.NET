using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NET4.TestClasses;
using PDNUtils.Help;
using PDNUtils.MultiThreadWorkflow;
using PDNUtils.Runner.Attributes;

namespace NET4.Parallel
{
    [RunableClass]
    public class ParallelTraverse
    {
        [Run(0)]
        protected void Go()
        {
            var tree = new MockEndlessTree();

            var node = new Node { Path = "root" };
            var sw = Stopwatch.StartNew();
            TraverseSingleThread(tree, node);
            sw.Stop();
            ConsolePrint.print("******************************  elapsed:{0}ms", sw.ElapsedMilliseconds);
            sw.Restart();
            TraverseMultiThread(tree, node);
            sw.Stop();
            ConsolePrint.print("******************************  elapsed:{0}ms", sw.ElapsedMilliseconds);
        }

        private void TraverseSingleThread(MockEndlessTree tree, NodeBase n)
        {
            //if (n is Node)
            //{
            //    ConsolePrint.print("Node: " + n.Path);
            //}
            //else
            //{
            //    ConsolePrint.print("Leaf: " + n.Path);
            //}

            var children = tree.GetChildNodes(n);

            foreach (var nodeBase in children)
            {
                TraverseSingleThread(tree, nodeBase);
            }
        }

        private static int maxConcurrentTasks = 6;

        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(maxConcurrentTasks, maxConcurrentTasks);

        private void TraverseMultiThread(MockEndlessTree tree, NodeBase n)
        {
            //if (n is Node)
            //{
            //    ConsolePrint.print("Node: " + n.Path);
            //}
            //else
            //{
            //    ConsolePrint.print("Leaf: " + n.Path);
            //}

            var children = tree.GetChildNodes(n);


            foreach (var nodeBase in children)
            {
                bool parallel = semaphoreSlim.Wait(0);

                if (parallel)
                {
                    var nn = nodeBase;
                    Task.Factory.StartNew(() => TraverseMultiThread(tree, nn)).ContinueWith((_) => semaphoreSlim.Release());
                }
                else
                {
                    TraverseMultiThread(tree, nodeBase);
                }
            }

        }

    }
}