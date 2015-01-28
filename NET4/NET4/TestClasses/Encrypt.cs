using System;
using System.Globalization;
using System.Text;
using System.Security.Cryptography;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    class Encrypt
    {
        public static String GetMD5HashString(String source)
        {
            byte[] data = Encoding.UTF8.GetBytes(source.ToCharArray());
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] buf = md5.ComputeHash(data);
            md5.Dispose();
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i < buf.Length; i++)
            {
                sb.Append(buf[i].ToString("X2"));
            }
            
            String hash = sb.ToString();
            return hash;
        }

        public static byte[] GetMD5Hash(String source)
        {
            byte[] data = Encoding.UTF8.GetBytes(source.ToCharArray());
            
            using(MD5 md5 = new MD5CryptoServiceProvider())
            {
                return md5.ComputeHash(data);
            }
        }


        public static byte[] GetBytesFromHashString(String hash)
        {
            int pos = 0;
            int arrpos = 0;
            byte[] arr = new byte[hash.Length / 2];
            Console.WriteLine("length " + hash.Length);
            while (pos < hash.Length - 1)
            {
                Console.WriteLine(hash.Substring(pos, 2));
                byte b = Byte.Parse(hash.Substring(pos, 2), NumberStyles.HexNumber);
                pos += 2;
                arr[arrpos++] = b;
            }
            return arr;
        }

        [Run(0)]
        public static void test()
        {
            String test = "test";
            Encrypt.GetBytesFromHashString("AFFA");
            Console.WriteLine("byte [" + "" + "]");
            String hash = Encrypt.GetMD5HashString(test);
            Console.WriteLine("init=[" + test + "] hash=[" + hash + "]");
            //String strFormat = String.Format("qqq\n{0}",10);
            //Console.WriteLine("format:["+strFormat+"]");

        }
    }
}