using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CompressLib
{
    // metadata container that we append to archived file to be able to decompress file in multithreaded way
    sealed class Footer
    {
        internal const int SizeOfLong = sizeof(long);

        public long[] ChunksStartPositions { get; } //positions of chunk's ends

        public int FooterSize { get; }

        public Footer(long[] chunksPositions)
        {
            if ((this.ChunksStartPositions = chunksPositions) == null) throw new ArgumentNullException(nameof(chunksPositions));
            FooterSize = SizeOfLong * (ChunksStartPositions.Length + 1);
        }

        public void AppendToStream(Stream stream)
        {
            List<byte> footerBytes = new List<byte>(FooterSize);//amount of position slots plus the count of them

            var bytesBuf = BitConverter.GetBytes((long)ChunksStartPositions.Length);
            footerBytes.AddRange(bytesBuf);

            foreach (var chunksPosition in ChunksStartPositions)
            {
                bytesBuf = BitConverter.GetBytes((long)chunksPosition);
                footerBytes.AddRange(bytesBuf);
            }

            footerBytes.Reverse();// reverse array to put the counter to the very end where we can find it easily and reverse back
            stream.Write(footerBytes.ToArray(), 0, FooterSize);
            stream.Flush();
        }

        public static Footer ReadFromStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanSeek) throw new ArgumentException("This instance of stream does not allow seek.", nameof(stream));
            var originaPosition = stream.Position;// remember original position
            stream.Seek(-SizeOfLong, SeekOrigin.End);// find the positions counter in the end of stream

            var buf = new byte[SizeOfLong];
            stream.Read(buf, 0, SizeOfLong);
            buf = buf.Reverse().ToArray();// reverse back to the normal order of bytes

            var count = BitConverter.ToInt64(buf, 0);
            var chunksPositions = new long[count];
            stream.Seek(-(long)(SizeOfLong * (count + 1)), SeekOrigin.End);

            // read positions
            for (long i = 0; i < (long)count; i++)
            {
                stream.Read(buf, 0, SizeOfLong);
                buf = buf.Reverse().ToArray();// reverse back to the normal order of bytes
                var positionEnd = BitConverter.ToInt64(buf, 0);
                chunksPositions[i] = positionEnd;
            }

            stream.Position = originaPosition;// return position to the original state
            return new Footer(chunksPositions.OrderBy(p => p).ToArray());
        }
    }
}