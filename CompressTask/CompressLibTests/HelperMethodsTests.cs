using System;
using System.Collections.Generic;
using CompressLib;
using NUnit.Framework;

namespace CompressLibTests
{
    [TestFixture]
    public class HelperMethodsTests
    {

        [TestCaseSource(nameof(AdjustChunkSizeCases))]
        public long GetChunkSizeTest(long totalSize, int threadsLimit) => HelperMethods.GetOptimalChunkSize(totalSize, threadsLimit);

        static IEnumerable<TestCaseData> AdjustChunkSizeCases()
        {
            var totalSize = 0;
            yield return new TestCaseData(totalSize, 8).Throws(typeof(ArgumentOutOfRangeException)).SetName($"Total: {totalSize} throws ArgumentOutOfRangeException");
            totalSize = 1;
            yield return new TestCaseData(totalSize, 8).Returns(SharedValues.MinChunkSize).SetName($"Total: {totalSize} Result is {nameof(SharedValues.MinChunkSize)}");
            totalSize = 100;
            yield return new TestCaseData(totalSize, 8).Returns(SharedValues.MinChunkSize).SetName($"Total: {totalSize} Result is {nameof(SharedValues.MinChunkSize)}");
            totalSize = 256;
            yield return new TestCaseData(totalSize, 8).Returns(SharedValues.MinChunkSize).SetName($"Total: {totalSize} Result is {nameof(SharedValues.MinChunkSize)}");
            totalSize = 257;
            yield return new TestCaseData(totalSize, 8).Returns(SharedValues.MinChunkSize).SetName($"Total: {totalSize} Result is {nameof(SharedValues.MinChunkSize)}");
            totalSize = 511;
            yield return new TestCaseData(totalSize, 8).Returns(SharedValues.MinChunkSize).SetName($"Total: {totalSize} Result is {nameof(SharedValues.MinChunkSize)}");
            totalSize = 512;
            yield return new TestCaseData(totalSize, 8).Returns(SharedValues.MinChunkSize).SetName($"Total: {totalSize} Result is {nameof(SharedValues.MinChunkSize)}");
            totalSize = 1024;
            yield return new TestCaseData(totalSize, 8).Returns(SharedValues.MinChunkSize).SetName($"Total: {totalSize} Result is {nameof(SharedValues.MinChunkSize)}");
            totalSize = 1024 * 1024;
            yield return new TestCaseData(totalSize, 1).Returns(totalSize).SetName($"Total: {totalSize} Result is {totalSize} (single thread)");
            totalSize = 1024 * 1024 * 20;
            yield return new TestCaseData(totalSize, 1).Returns(SharedValues.MaxChunkSize).SetName($"Total: {totalSize} Result is {SharedValues.MaxChunkSize} (single thread)");
            totalSize = 1024 * 512;
            yield return new TestCaseData(totalSize, 8).Returns(40960).SetName($"Total: {totalSize} Result is {40960}");
            totalSize = 1024 * 1024 * 10;
            yield return new TestCaseData(totalSize, 8).Returns(1310720).SetName($"Total: {totalSize} Result is {1310720}");
        }

        [TestCaseSource(nameof(SplitTestCases))]
        ICollection<ChunkPosition> SplitSizeToChunks_Returns_Expected_Collection(long size, long chunkSize) => HelperMethods.BuildChunksPositions(size, chunkSize);

        static IEnumerable<TestCaseData> SplitTestCases()
        {
            var totalSize = 0;
            var chunkSize = 1;
            yield return new TestCaseData(totalSize, chunkSize).Throws(typeof(ArgumentOutOfRangeException)).SetName($"Total: {totalSize}, chunk: {chunkSize} throws ArgumentOutOfRangeException");
            totalSize = 1;
            chunkSize = 0;
            yield return new TestCaseData(totalSize, chunkSize).Throws(typeof(ArgumentOutOfRangeException)).SetName($"Total: {totalSize}, chunk: {chunkSize} throws ArgumentOutOfRangeException");
            totalSize = 0;
            chunkSize = 0;
            yield return new TestCaseData(totalSize, chunkSize).Throws(typeof(ArgumentOutOfRangeException)).SetName($"Total: {totalSize}, chunk: {chunkSize} throws ArgumentOutOfRangeException");
            totalSize = 1;
            chunkSize = 2;
            yield return new TestCaseData(totalSize, chunkSize).Returns(new ChunkPosition[]
            {
                new ChunkPosition(0, 0, 0),
            }).SetName($"Total: {totalSize}, chunk: {chunkSize}");
            totalSize = 5;
            chunkSize = 3;
            yield return new TestCaseData(totalSize, chunkSize).Returns(new ChunkPosition[]
            {
                new ChunkPosition(0, 0, 2),
                new ChunkPosition(1, 3, 4),
            }).SetName($"Total: {totalSize}, chunk: {chunkSize}");
            totalSize = 10;
            chunkSize = 2;
            yield return new TestCaseData(totalSize, chunkSize).Returns(new ChunkPosition[]
            {
                new ChunkPosition(0, 0, 1),
                new ChunkPosition(1, 2, 3),
                new ChunkPosition(2, 4, 5),
                new ChunkPosition(3, 6, 7),
                new ChunkPosition(4, 8, 9),
            }).SetName($"Total: {totalSize}, chunk: {chunkSize}");
            totalSize = 11;
            chunkSize = 2;
            yield return new TestCaseData(totalSize, chunkSize).Returns(new ChunkPosition[]
            {
                new ChunkPosition(0, 0, 1),
                new ChunkPosition(1, 2, 3),
                new ChunkPosition(2, 4, 5),
                new ChunkPosition(3, 6, 7),
                new ChunkPosition(4, 8, 9),
                new ChunkPosition(5, 10, 10),
            }).SetName($"Total: {totalSize}, chunk: {chunkSize}");
        }
    }
}