using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CompressLib.Exceptions;

namespace CompressLib
{
    // provides abstraction for building collection of chunks for original and compressed files
    sealed class FileChunksCollectionBuilder
    {
        private readonly int _parallelLimit;
        private readonly FileInfo _fileInfo;

        public FileChunksCollectionBuilder(FileInfo fileInfo, int parallelLimit)
        {
            if ((_fileInfo = fileInfo) == null) throw new ArgumentNullException(nameof(fileInfo));
            if (parallelLimit < 1) throw new ArgumentOutOfRangeException(nameof(parallelLimit), parallelLimit, "Must be greater than 0.");
            if (!_fileInfo.Exists) throw new FileNotFoundException($"File not found [{_fileInfo.FullName}]. Check existence and permissions.");
            if (_fileInfo.Length == 0) throw new EmptyFileException($"File [{_fileInfo.FullName}] is empty.");
            _parallelLimit = parallelLimit;
        }

        public FileChunk[] BuildFileChunksCollectionForCompress()
        {
            var totalSize = _fileInfo.Length;
            var bestChunkSize = HelperMethods.GetOptimalChunkSize(totalSize, _parallelLimit);
            return HelperMethods.BuildChunksPositions(totalSize, bestChunkSize).Select(chunkPos => new FileChunk(new FileInfo(_fileInfo.FullName), chunkPos)).ToArray();
        }

        public FileChunk[] BuildFileChunksCollectionForDecompress()
        {
            Footer footer;

            using (var footerStream = _fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                footer = Footer.ReadFromStream(footerStream);
            }

            IList<FileChunk> fileChunks = new List<FileChunk>(footer.ChunksStartPositions.Length + 1);
            long order = 0;
            long startPosition = 0;

            foreach (var nextChunkStartPosition in footer.ChunksStartPositions)
            {
                fileChunks.Add(new FileChunk(new FileInfo(_fileInfo.Name), new ChunkPosition(order++, startPosition, nextChunkStartPosition - 1)));
                startPosition = nextChunkStartPosition;
            }

            // we can calculate the last filechunk end position, hence we do not write it to header
            fileChunks.Add(new FileChunk(new FileInfo(_fileInfo.FullName), new ChunkPosition(order++, startPosition, _fileInfo.Length - footer.FooterSize)));
            return fileChunks.ToArray();
        }
    }
}