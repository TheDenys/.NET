using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using CompressLib;
using NUnit.Framework;

namespace CompressLibTests
{
    [TestFixture]
    public class CompressDecompressTests
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

        [TestCaseSource(nameof(RoundtripCaseSources))]
        public void RoundTrip_Compression_Should_Produce_Original_File(string originFileName, int compressThreadsCount, int decompressThreadsCount)
        {
            var compressedFileInfo = new FileInfo(originFileName + ".gz");
            var decompressedFileInfo = new FileInfo(originFileName + ".decompressed");

            try
            {
                var sourceFileInfo = new FileInfo(originFileName);
                MultithreadGZip.ProcessFileInternal(CompressionMode.Compress, sourceFileInfo, compressedFileInfo, compressThreadsCount);
                MultithreadGZip.ProcessFileInternal(CompressionMode.Decompress, compressedFileInfo, decompressedFileInfo, decompressThreadsCount);
                FileAssert.AreEqual(sourceFileInfo, decompressedFileInfo, "Decompressed files content is different from it's origin.");
            }
            finally
            {
                // cleanup
                compressedFileInfo.Delete();
                decompressedFileInfo.Delete();
            }
        }

        IEnumerable<TestCaseData> RoundtripCaseSources()
        {
            var originName = UnitTestConstants.File1Mb;
            var compressThreadsLimit = 1;
            var decompressThreadsLimit = 1;
            yield return new TestCaseData(originName, compressThreadsLimit, decompressThreadsLimit).SetName($"Max. compress threads: {compressThreadsLimit}, max. decompress threads: {decompressThreadsLimit} file: [{originName}]");

            originName = UnitTestConstants.File1Mb;
            compressThreadsLimit = 1;
            decompressThreadsLimit = 2;
            yield return new TestCaseData(originName, compressThreadsLimit, decompressThreadsLimit).SetName($"Max. compress threads: {compressThreadsLimit}, max. decompress threads: {decompressThreadsLimit} file: [{originName}]");

            originName = UnitTestConstants.File1Mb;
            compressThreadsLimit = 2;
            decompressThreadsLimit = 1;
            yield return new TestCaseData(originName, compressThreadsLimit, decompressThreadsLimit).SetName($"Max. compress threads: {compressThreadsLimit}, max. decompress threads: {decompressThreadsLimit} file: [{originName}]");

            originName = UnitTestConstants.File1Mb;
            compressThreadsLimit = 10;
            decompressThreadsLimit = 10;
            yield return new TestCaseData(originName, compressThreadsLimit, decompressThreadsLimit).SetName($"Max. compress threads: {compressThreadsLimit}, max. decompress threads: {decompressThreadsLimit} file: [{originName}]");

            originName = UnitTestConstants.File10Mb;
            compressThreadsLimit = 10;
            decompressThreadsLimit = 10;
            yield return new TestCaseData(originName, compressThreadsLimit, decompressThreadsLimit).SetName($"Max. compress threads: {compressThreadsLimit}, max. decompress threads: {decompressThreadsLimit} file: [{originName}]");

            originName = UnitTestConstants.File1Mb;
            compressThreadsLimit = 20;
            decompressThreadsLimit = 20;
            yield return new TestCaseData(originName, compressThreadsLimit, decompressThreadsLimit).SetName($"Max. compress threads: {compressThreadsLimit}, max. decompress threads: {decompressThreadsLimit} file: [{originName}]");
        }
    }
}
