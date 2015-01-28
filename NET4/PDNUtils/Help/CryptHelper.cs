using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PDNUtils.Help
{
    /// <summary>
    /// CryptHelper allows to encrypt/decrypt text
    /// </summary>
    public static class CryptHelper
    {
        private static byte[] salt = new byte[] {0x14, 0x51, 0xdf, 0x43,
                                                 0xa0, 0x9a, 0x1e, 0x91,
                                                 0x72, 0x8a, 0x61, 0x52,
                                                 0x70, 0x14, 0x7e, 0xf9};

        private static SymmetricAlgorithm GetAlgorithm()
        {
            Rijndael alg = Rijndael.Create();
            var pdb = new Rfc2898DeriveBytes("passwordпроизвольный", salt);
            alg.Key = pdb.GetBytes(alg.KeySize / 8);
            alg.IV = pdb.GetBytes(alg.BlockSize / 8);
            return alg;
        }

        /// <summary>
        /// Encrypts/decrypts given text
        /// </summary>
        /// <param name="clearText">text for encryption/decryption</param>
        /// <param name="encrypt">true - encrypt, false - decrypt</param>
        /// <returns>encrypted/decryped text</returns>
        public static string Encrypt(string clearText, bool encrypt)
        {
            var clearBytes = encrypt ? Encoding.Unicode.GetBytes(clearText) : Convert.FromBase64String(clearText);
            var ms = new MemoryStream();
            var alg = GetAlgorithm();
            var cs = new CryptoStream(ms, encrypt ? alg.CreateEncryptor() : alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(clearBytes, 0, clearBytes.Length);
            cs.Close();
            var res = new StringBuilder(encrypt ? Convert.ToBase64String(ms.ToArray()) : Encoding.Unicode.GetString(ms.ToArray()));
            return res.ToString();
        }

    }
}
