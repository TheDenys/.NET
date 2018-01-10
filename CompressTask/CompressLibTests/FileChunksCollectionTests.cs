using System;
using System.IO;
using System.Linq;
using System.Threading;
using CompressLib;
using NUnit.Framework;

namespace CompressLibTests
{
    [TestFixture]
    public class FileChunksCollectionTests
    {
        [TestCase(1, 0, TestName = "1 thread 0 items")]
        [TestCase(1, 10, TestName = "1 thread 10 items")]
        [TestCase(10, 1, TestName = "10 threads 1 item")]
        [TestCase(4, 10, TestName = "4 threads 10 items")]
        [TestCase(10, 10, TestName = "10 threads 10 items")]
        public void Consuming_From_Multiple_Threads_Processes_All_Items(int threadsCount, int itemsCount)
        {
            int processedItems = 0;

            var sut = new FileChunksCollection(Enumerable.Range(0, itemsCount).Select(p => new FileChunk(new FileInfo("."), new ChunkPosition(p, 0, 0))).ToArray());

            Action processChunks = () =>
            {
                FileChunk fileChunk;
                while (sut.GetNextChunk(out fileChunk))
                {
                    Interlocked.Increment(ref processedItems);
                }
            };

            Thread[] threads = Enumerable.Range(0, threadsCount).Select(tNum => new Thread(() => processChunks())).ToArray();

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join(TimeSpan.FromSeconds(60));// 60 seconds should be more than enough to finish the test task
            }
        }

    }

    sealed class FileChunksCollection
    {
        private readonly object _locker = new object();

        private long _chunkPosition;
        private FileChunk[] Chunks { get; }
        public long TotalChunks { get; }

        public FileChunksCollection(FileChunk[] fileChunks)
        {
            if ((Chunks = fileChunks) == null) throw new ArgumentNullException(nameof(fileChunks));
            TotalChunks = fileChunks.Length;
        }

        public bool GetNextChunk(out FileChunk fileChunk)
        {
            lock (_locker)
            {
                if (_chunkPosition >= TotalChunks)
                {
                    fileChunk = null;
                    return false;
                }

                fileChunk = Chunks[_chunkPosition++];
                return true;
            }
        }
    }
}