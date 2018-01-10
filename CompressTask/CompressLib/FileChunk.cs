using System;
using System.IO;

namespace CompressLib
{
    // metadata and content (compressed/decompressed bytes) of chunk of a file that we will use to write to resulting file
    class FileChunk : IEquatable<FileChunk>
    {
        public FileInfo FileInfo { get; }
        public ChunkPosition ChunkPosition { get; }
        public MemoryStream ContentMemoryStream { get; set; }

        public FileChunk(FileInfo fileInfo, ChunkPosition chunkPosition)
        {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));
            FileInfo = fileInfo;
            ChunkPosition = chunkPosition;
        }

        public override int GetHashCode()
        {
            return ChunkPosition.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is FileChunk && Equals((FileChunk)obj);
        }

        public bool Equals(FileChunk other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other == null) return false;
            return ChunkPosition.Equals(other.ChunkPosition);
        }
    }
}