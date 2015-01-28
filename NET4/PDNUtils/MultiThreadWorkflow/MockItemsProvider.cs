using System.Collections.Generic;
using System.Threading;

namespace PDNUtils.MultiThreadWorkflow
{
    public class MockItemsProvider : IItemsProvider<string>
    {

        private readonly int max;

        public MockItemsProvider(int max)
        {
            this.max = max;
        }

        public IEnumerable<string> GetItems(CancellationToken cancel)
        {
            for (int i = 0; i < max && !cancel.IsCancellationRequested; i++)
            {
                yield return "Task " + i;
            }
        }
    }
}