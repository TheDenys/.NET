using System;
using System.Security.Cryptography;

namespace PDNUtils.Help
{
    public static class RandomNumber
    {
        private static readonly RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

        public static int Next(int min, int max)
        {
            var d = max - min + 1;
            var randomNumber = new byte[8];
            provider.GetBytes(randomNumber);
            int v = (int)(Math.Abs(BitConverter.ToInt64(randomNumber, 0)) % d) + min;
            return v;
        }
    }
}
