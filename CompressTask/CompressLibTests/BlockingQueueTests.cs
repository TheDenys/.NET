using System;
using System.Threading;
using CompressLib;
using NUnit.Framework;

namespace CompressLibTests
{
    [TestFixture]
    public class BlockingQueueTests
    {
        [Test]
        public void Deque_Keeps_Initial_Order()
        {
            using (var sut = new BlockingQueue<int>())
            {
                sut.Enqueue(new[] { 1, 2, 3 });
                int dequeuedItem;
                sut.TryDequeue(out dequeuedItem);
                Assert.AreEqual(1, dequeuedItem);
                sut.TryDequeue(out dequeuedItem);
                Assert.AreEqual(2, dequeuedItem);
                sut.TryDequeue(out dequeuedItem);
                Assert.AreEqual(3, dequeuedItem);
            }
        }

        [Test]
        public void Finish_Adding_Releases_All_Waiting_Readers()
        {
            using (var sut = new BlockingQueue<int>())
            {
                int fetched = 0;
                AutoResetEvent are = new AutoResetEvent(false);
                AutoResetEvent are2 = new AutoResetEvent(false);

                var readerThread = new Thread(() =>
                {
                    are.Set();
                    int item1;
                    if (sut.TryDequeue(out item1)) Interlocked.Increment(ref fetched);
                });
                readerThread.Start();

                var readerThread2 = new Thread(() =>
                {
                    are2.Set();
                    int item1;
                    if (sut.TryDequeue(out item1)) Interlocked.Increment(ref fetched);
                });
                readerThread2.Start();

                // making sure the reader threads have started
                Assert.True(are.WaitOne(TimeSpan.FromSeconds(20)));
                Assert.True(are2.WaitOne(TimeSpan.FromSeconds(20)));

                sut.Enqueue(new[] { 1 });
                sut.FinishAdding();

                Assert.True(readerThread.Join(TimeSpan.FromSeconds(20)));
                Assert.True(readerThread2.Join(TimeSpan.FromSeconds(20)));

                Assert.AreEqual(1, fetched);
            }
        }
    }
}