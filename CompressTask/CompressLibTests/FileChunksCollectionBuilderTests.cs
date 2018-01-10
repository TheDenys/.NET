using System.IO;
using CompressLib;
using NUnit.Framework;

namespace CompressLibTests
{
    [TestFixture]
    public class FileChunksCollectionBuilderTests
    {
        [SetUp]
        public void CreateFiles()
        {
            UnitTestHelpers.CreateFile(UnitTestConstants.File1Mb, UnitTestConstants.Size1Mb);
            UnitTestHelpers.CreateFile(UnitTestConstants.File10Mb, UnitTestConstants.Size10Mb);
        }

        [TearDown]
        public void DeleteFiles()
        {
            File.Delete(UnitTestConstants.File1Mb);
            File.Delete(UnitTestConstants.File10Mb);
        }

        [TestCase(UnitTestConstants.File1Mb, 13)]
        [TestCase(UnitTestConstants.File10Mb, 8)]
        public void BuildFileChunksCollection(string fileName, int expectedChunksCount)
        {
            var sut = new FileChunksCollectionBuilder(new FileInfo(fileName), 8);
            var res = sut.BuildFileChunksCollectionForCompress();
            var order = 0;

            foreach (FileChunk chunk in res)
            {
                Assert.AreEqual(order++, chunk.ChunkPosition.Order, "Chunk order mismatch.");
            }

            Assert.AreEqual(expectedChunksCount, order, "Expected chunks number mismatch.");
        }
    }
}