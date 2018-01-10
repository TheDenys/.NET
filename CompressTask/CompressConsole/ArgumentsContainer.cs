using System;
using System.IO;
using System.IO.Compression;

namespace CompressConsole
{
    public class ArgumentsContainer
    {
        public CompressionMode CompressionMode { get; }
        public FileInfo OriginFileName { get; }
        public FileInfo ResultFileName { get; }

        public ArgumentsContainer(CompressionMode compressionMode, string originFileName, string resultFileName)
        {
            if (string.IsNullOrEmpty(originFileName)) throw new ArgumentNullException(nameof(originFileName));
            if (string.IsNullOrEmpty(resultFileName)) throw new ArgumentNullException(nameof(resultFileName));
            CompressionMode = compressionMode;
            OriginFileName = new FileInfo(originFileName);
            ResultFileName = new FileInfo(resultFileName);
        }
    }
}