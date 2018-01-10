using System;
using System.Collections.Generic;

namespace CompressLib
{
    class HelperMethods
    {
        public static long GetOptimalChunkSize(long totalSize, int minimumAmountLimit, int minChunkSize = SharedValues.MinChunkSize, int maxChunkSize = SharedValues.MaxChunkSize)
        {
            if (totalSize <= 0) throw new ArgumentOutOfRangeException(nameof(totalSize));

            var chunkSize = maxChunkSize;

            if (minimumAmountLimit == 1 && totalSize < chunkSize) return totalSize;

            // looking for chunksize that will let us split file at least at amount of allowed parallel tasks so we can keep all CPUs busy
            while (chunkSize > minChunkSize)
            {
                if (totalSize / chunkSize >= minimumAmountLimit) return chunkSize;
                var temp = chunkSize / 2;
                if (temp < minChunkSize) return minChunkSize;
                chunkSize = temp;
            }

            return chunkSize;
        }

        // builds collection of chunk positions in the file
        public static ICollection<ChunkPosition> BuildChunksPositions(long totalSize, long chunkSize)
        {
            if (totalSize <= 0) throw new ArgumentOutOfRangeException(nameof(totalSize));
            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
            long rows = (int)Math.Ceiling((double)totalSize / chunkSize);
            long lastCount = totalSize % chunkSize;
            long currentChunksize = chunkSize;

            long currentStart = 0;
            long currentEnd = 0;

            ICollection<ChunkPosition> res = new List<ChunkPosition>();
            for (int i = 0; i < rows; i++)
            {
                if (lastCount != 0 && i == rows - 1) currentChunksize = lastCount;
                currentEnd = currentStart + currentChunksize - 1;
                res.Add(new ChunkPosition(i, currentStart, currentEnd));
                currentStart = currentEnd + 1;
            }

            return res;
        }
    }
}