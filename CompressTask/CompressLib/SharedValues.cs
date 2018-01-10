using System;

namespace CompressLib
{
    class SharedValues
    {
        public const int MinChunkSize = 256;// let's assume it's no worth splitting files that are smaller than 256 bytes because metadata will add significant part of resulting size
        public const int MaxChunkSize = 10 * 1024 * 1024;//10Mb, should be safe enough for machines with low memory where we unpack the file
        public static int ThreadsLimit { get; } = Environment.ProcessorCount;// usually windows handles well amount of threads that is equal to amount of CPU cores
    }
}