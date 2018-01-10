using System.IO;
using CompressLib;
using NUnit.Framework;

namespace CompressLibTests
{
    [TestFixture]
    public class FooterTests
    {
        [Test]
        public void Footer_From_Stream_Is_The_Same_As_Original()
        {
            Footer f = new Footer(new long[] { 50, 100, 200 });
            var memoryStream = new MemoryStream();
            f.AppendToStream(memoryStream);
            var f2 = Footer.ReadFromStream(memoryStream);
            Assert.AreEqual(3, f.ChunksStartPositions.Length);
            CollectionAssert.AreEqual(new ulong[] { 50, 100, 200 }, f2.ChunksStartPositions);
        }
    }
}