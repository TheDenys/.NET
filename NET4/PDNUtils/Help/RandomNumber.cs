using System;
using System.Security.Cryptography;

namespace PDNUtils.Help
{
    public static class RandomNumber
    {
        private static readonly RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

        public static int Next(int min, int max)
        {
            // get delta that will be used for modulo operation
            var d = max - min + 1;
            // get "random" bytes into buffer
            var randomNumber = new byte[4];
            provider.GetBytes(randomNumber);
            // convert bytes into number 
            // then get modulo by delta from first step
            // and add lower limit (min) to make the number fit within desired range
            int v = (Math.Abs(BitConverter.ToInt32(randomNumber, 0)) % d) + min;
            return v;
        }
    }
}
