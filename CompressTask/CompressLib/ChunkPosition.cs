using System;

namespace CompressLib
{
    // this metadata lets us write pieces of file to exact positions
    // further improvement could be done for application - combine resulting file not in single thread but in multiple
    // though on HT quad core even this implementation is almost 10 times faster than ordinary GZip compressor
    struct ChunkPosition : IEquatable<ChunkPosition>
    {
        public long Order { get; }
        public long Start { get; }
        public long End { get; }

        public ChunkPosition(long order, long start, long end)
        {
            if (order < 0) throw new ArgumentOutOfRangeException(nameof(order), "Must be non less than 0.");
            if (start < 0) throw new ArgumentOutOfRangeException(nameof(start), "Must be non less than 0.");
            if (end < start) throw new ArgumentOutOfRangeException(nameof(end), $"Must be greater or equal to {nameof(start)} parameter.");
            Order = order;
            Start = start;
            End = end;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(Order ^ Start ^ End);
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is ChunkPosition) && Equals((ChunkPosition)obj);
        }

        public bool Equals(ChunkPosition other)
        {
            return Order == other.Order && Start == other.Start && End == other.End;
        }
    }
}