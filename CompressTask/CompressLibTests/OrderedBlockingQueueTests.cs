using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CompressLib;
using NUnit.Framework;

namespace CompressLibTests
{
    [TestFixture]
    public class OrderedBlockingQueueTests
    {
        int limit = 24;

        [TestCaseSource(nameof(ProduceConsumeCases))]
        public void Queue_Should_Proceed_With_Any_Amount_Of_Producing_Threads_And_Items(int threadsCount, int totlaItems)
        {
            using (OrderedBlockingQueue<FileChunk> sut = new OrderedBlockingQueue<FileChunk>())
            {
                var currentItem = 0;

                var items = Enumerable.Range(0, totlaItems).Select(order => new FileChunk(new FileInfo("."), new ChunkPosition(order, 0, 0))).ToArray();
                var fileChunksCollection = new FileChunksCollection(items);

                Action produceChunks = () =>
                {
                    FileChunk fileChunk;
                    while (fileChunksCollection.GetNextChunk(out fileChunk))
                    {
                        sut.Enqueue(fileChunk.ChunkPosition.Order, fileChunk);
                    }
                };

                Thread[] chunkProducingThreads = Enumerable.Range(0, threadsCount).Select(tNum => new Thread(() => produceChunks())).ToArray();

                int processedItems = 0;
                var threadReader = new Thread(() =>
                {
                    FileChunk item;
                    while (sut.TryDequeue(out item))
                    {
                        Assert.AreEqual(processedItems, item.ChunkPosition.Order);
                        Interlocked.Increment(ref processedItems);
                    }

                    System.Diagnostics.Trace.WriteLine("FINISHED CONSUMING");
                });

                foreach (var thread in chunkProducingThreads) thread.Start();
                threadReader.Start();

                foreach (var thread in chunkProducingThreads)
                {
                    thread.Join(TimeSpan.FromMinutes(5));// timeout lets us avoid hanging build agent forever if stuff happens
                }

                System.Diagnostics.Trace.WriteLine("FINISHED PRODUCING");

                sut.FinishAdding();

                threadReader.Join(TimeSpan.FromMinutes(5));

                Assert.AreEqual(totlaItems, processedItems);
            }
        }

        IEnumerable<TestCaseData> ProduceConsumeCases()
        {
            for (var t = 1; t <= 16; t++)
            {
                for (int i = t - 1; i <= t * 2; i++)
                    yield return new TestCaseData(t, i).SetName($"Produce and consume {i:000} item(s) with {t:00} threads");
            }
        }

        [Test]
        public void AddItem_Throws_Exception_If_Item_With_The_Same_Order_Already_Exists()
        {
            var sut = new OrderedBlockingQueue<int>();
            sut.Enqueue(0, 1);
            Assert.Throws<ArgumentException>(() => sut.Enqueue(0, 1));
        }
    }
}