using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CompressLib;
using NUnit.Framework;

namespace CompressLibTests
{
    [TestFixture]
    public class ItemsProcessorTests
    {
        [TestCaseSource(nameof(ProcessItemsCases))]
        public void ProcessItems(int itemsCount, int threadsCount)
        {
            object _locker = new object();
            var itemsForProcessing = Enumerable.Range(1, itemsCount).ToList();
            var processedItems = new List<int>();
            var postProcessedItems = new List<int>();

            var sut = new ItemsProcessor<int>(
                itemsForProcessing,
                item =>
                {
                    // here we need lock as there are several processing threads
                    lock (_locker) processedItems.Add(item);
                },
                item =>
                {
                    // no lock needed as there's a single post-processing thread
                    postProcessedItems.Add(item);
                },
                threadsCount
                );

            var t = new Thread(() => sut.ProcessItems());
            t.Start();
            t.Join();

            //Assert.True(t.Join(TimeSpan.FromSeconds(20)), "Processing hasn't finished within expected time.");
            CollectionAssert.AreEquivalent(itemsForProcessing, processedItems, "Expected processed items do not match the actual ones.");
            CollectionAssert.AreEqual(itemsForProcessing, postProcessedItems, "Expected post-processed collection should be the same as initial one.");
        }

        static IEnumerable<TestCaseData> ProcessItemsCases()
        {
            yield return new TestCaseData(0, 0).Throws(typeof(ArgumentOutOfRangeException)).SetName("0 threads - throws exception");

            int threadsCount;
            int itemsCount;

            yield return new TestCaseData(0, 4).SetName($"Empty queue with 4 threads");

            for (threadsCount = 1; threadsCount <= 4; threadsCount++)
            {
                for (itemsCount = 1; itemsCount < 4; itemsCount++)
                    yield return new TestCaseData(itemsCount, threadsCount).SetName($"{itemsCount} items, {threadsCount} threads");
            }

            for (threadsCount = 8; threadsCount <= 32; threadsCount += 8)
            {
                itemsCount = 4000;
                yield return new TestCaseData(itemsCount, threadsCount).SetName($"{itemsCount} items, {threadsCount} threads");
            }

            itemsCount = 8;
            threadsCount = 32;
            yield return new TestCaseData(itemsCount, threadsCount).SetName($"More threads than items: {itemsCount} items, {threadsCount} threads");
        }
    }
}
