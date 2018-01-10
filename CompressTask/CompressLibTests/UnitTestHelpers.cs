using System.Collections.Generic;
using System.IO;

namespace CompressLibTests
{
    public class UnitTestHelpers
    {
        public static void CreateFile(string fileName, long sizeBytes)
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                foreach (var b in GetBytesSequence(sizeBytes))
                {
                    fs.WriteByte(b);
                }
                fs.Flush();
                fs.Close();
            }
        }

        public static IEnumerable<byte> GetBytesSequence(long amount)
        {
            byte b = 0;

            while (amount-- > 0)
            {
                unchecked
                {
                    yield return b++;
                }
            }
        }
    }
}