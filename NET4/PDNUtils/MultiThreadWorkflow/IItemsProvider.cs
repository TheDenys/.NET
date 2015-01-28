using System.Collections.Generic;
using System.Threading;

namespace PDNUtils.MultiThreadWorkflow
{
    public interface IItemsProvider<out T>
    {
        IEnumerable<T> GetItems(CancellationToken cancel);

        /*void Start();

        void ReportNewItem(T item);

        void ReportLastItem();*/
    }
}