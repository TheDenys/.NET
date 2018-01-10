using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace CompressLib
{
    /// <summary>
    /// Class provifind methods for high performant GZip (de)compression of files.
    /// </summary>
    public static class MultithreadGZip
    {
        public static void ProcessFile(CompressionMode mode, FileInfo sourceFileInfo, FileInfo targetFileInfo)
        {
            ProcessFileInternal(mode, sourceFileInfo, targetFileInfo, SharedValues.ThreadsLimit);
        }

        public static void ProcessFile(CompressionMode mode, FileInfo sourceFileInfo, FileInfo targetFileInfo, int threadsCount)
        {
            ProcessFileInternal(mode, sourceFileInfo, targetFileInfo, threadsCount);
        }

        internal static void ProcessFileInternal(CompressionMode mode, FileInfo sourceFileInfo, FileInfo targetFileInfo, int threadsCount)
        {
            if (sourceFileInfo == null) throw new ArgumentNullException(nameof(sourceFileInfo));
            if (targetFileInfo == null) throw new ArgumentNullException(nameof(targetFileInfo));
            if (threadsCount < 1) throw new ArgumentOutOfRangeException(nameof(threadsCount), threadsCount, "Must be a positive number.");

            FileChunksCollectionBuilder fcb = new FileChunksCollectionBuilder(sourceFileInfo, threadsCount);
            FileChunk[] fileChunks;
            Action<FileChunk> processChunk;

            switch (mode)
            {
                case CompressionMode.Compress:
                    fileChunks = fcb.BuildFileChunksCollectionForCompress();
                    processChunk = CompressChunkToMemoryStream;
                    break;
                case CompressionMode.Decompress:
                    fileChunks = fcb.BuildFileChunksCollectionForDecompress();
                    processChunk = DeCompressChunkToMemoryStream;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown mode.");
            }

            var totalChunks = fileChunks.LongLength;
            var currentItem = 0;
            int processedItems = 0;

            using (var targetFileStream = targetFileInfo.Open(FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                var positions = new List<long>();
                long lastPosition = 0;

                Action<FileChunk> saveChunk = chunk =>
                {
                    using (var memoryStream = chunk.ContentMemoryStream)
                    {
                        memoryStream.Position = 0;
                        var buf = new byte[1024];
                        int readBytes;

                        while ((readBytes = memoryStream.Read(buf, 0, buf.Length)) != 0)
                        {
                            targetFileStream.Write(buf, 0, readBytes);
                        }
                    }

                    lastPosition = targetFileStream.Position;
                    positions.Add(lastPosition);
                    Interlocked.Increment(ref processedItems);
                };

                var sut = new ItemsProcessor<FileChunk>(
                    fileChunks,
                    item => processChunk(item),
                    item => saveChunk(item),
                    threadsCount);

                sut.ProcessItems();

                // there's no need to remember the last one because it points to the start of footer
                positions.Remove(lastPosition);

                // in compression mode save footer to the end of file
                // in such a way standard tools may unpack it (at least TotalCommander can)
                if (mode == CompressionMode.Compress)
                {
                    Footer f = new Footer(positions.ToArray());
                    f.AppendToStream(targetFileStream);
                    targetFileStream.Flush();
                }
            }
        }

        static void CompressChunkToMemoryStream(FileChunk fileChunk)
        {
            if (fileChunk == null) throw new ArgumentNullException(nameof(fileChunk));
            var chunkPosition = fileChunk.ChunkPosition;
            var bytesToReadFromSource = chunkPosition.End - chunkPosition.Start + 1;// position start 0, end 0 still has 1 byte, hence plus one
            MemoryStream compressedFs = new MemoryStream();

            using (var sourceFs = fileChunk.FileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                sourceFs.Position = chunkPosition.Start;

                var positions = new List<long>();

                using (GZipStream compressionStream = new GZipStream(compressedFs, CompressionMode.Compress, true))
                {
                    byte[] buf = new byte[1024];
                    var bytesToReadNext = (int)Math.Min(buf.Length, bytesToReadFromSource);
                    bytesToReadFromSource -= bytesToReadNext;

                    int readBytes = 0;

                    while (bytesToReadNext > 0 && (readBytes = sourceFs.Read(buf, 0, bytesToReadNext)) != 0)
                    {
                        compressionStream.Write(buf, 0, readBytes);
                        bytesToReadNext = (int)Math.Min(buf.Length, bytesToReadFromSource);
                        bytesToReadFromSource -= bytesToReadNext;
                    }

                    compressionStream.Flush();
                }

                compressedFs.Flush();
                positions.Add(compressedFs.Length);
            }

            fileChunk.ContentMemoryStream = compressedFs;
        }

        static void DeCompressChunkToMemoryStream(FileChunk fileChunk)
        {
            if (fileChunk == null) throw new ArgumentNullException(nameof(fileChunk));
            var chunkPosition = fileChunk.ChunkPosition;
            //var bytesToReadFromSource = chunkPosition.End - chunkPosition.Start + 1;// position start 0, end 0 still has 1 byte, hence plus one
            MemoryStream decompressedMs = new MemoryStream();

            using (var compressedFs = fileChunk.FileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                compressedFs.Position = chunkPosition.Start;// set position to the beginning of the compressed block

                using (GZipStream decompressionStream = new GZipStream(compressedFs, CompressionMode.Decompress, true))
                {
                    byte[] buf = new byte[1024];
                    int readBytes = 0;

                    while ((readBytes = decompressionStream.Read(buf, 0, buf.Length)) != 0)
                    {
                        decompressedMs.Write(buf, 0, readBytes);
                    }

                    decompressedMs.Flush();
                }
            }

            fileChunk.ContentMemoryStream = decompressedMs;
        }
    }
}
