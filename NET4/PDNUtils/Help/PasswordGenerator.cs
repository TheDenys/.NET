using System;
using System.Collections.Generic;

namespace PDNUtils.Help
{

    /// <summary>
    /// builder for <see cref="PasswordGenerator"/>
    /// </summary>
    public static class PasswordGeneratorBuilder
    {
        // shows the minimal amount of characters from appropriate alphabite to be used in password
        private static int[] arrObligateQuantity = { 1, 1, 1, 0 };

        // alphabites for generating the password
        private static char[][] arrAlphabites = new[]
                                                    {
                new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x' },
                new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X' },
                new[] { '2', '3', '4', '5', '6', '7', '8', '9' },
                new[] { '!', '?', '@', '#', '$', '%', '^', '&', '*', '+', '/', '=' },
                                                    };

        private static PasswordGenerator _instance;
        private static readonly object locker = new object();

        public static PasswordGenerator Instance
        {
            get
            {
                lock (locker)
                {
                    if (_instance == null)
                        _instance = new PasswordGenerator(arrObligateQuantity, arrAlphabites, 8, 8);
                }
                return _instance;
            }
        }
    }

    // additional class for generating the random passwords against specified requirements
    public class PasswordGenerator
    {
        // Create a logger for use in this class
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private char[] chArrAlphabite;
        private uint min_length, max_length;
        private Random r;

        private int[] arrIObligateQuantity;
        private char[][] chArrAlphabites;

        public PasswordGenerator(int[] arrIObligateQuantity, char[][] chArrAlphabites, uint minLength, uint maxLength)
        {
            //bool bValidAlphabite = true;
            //bool bValidObligateChars = true;
            //bool bValidObligateQuantity = true;
            //string err_mess = string.Empty;
            //err_mess += (bValidAlphabite = validateSequence(chArrAlphabite)) ? string.Empty : "Alphabite is invalid. ";
            //err_mess += (bValidObligateChars = validateSequence(chArrObligateCharacters)) ? string.Empty : "Obligate chars sequence is invalid. ";
            //err_mess += (bValidObligateQuantity = Math.Max(min_length, max_length) >= iObligateQuantity) ? string.Empty : "Incorrect password length range.";
            //if (!(bValidAlphabite && bValidObligateChars && bValidObligateQuantity))
            //{
            //    throw new ArgumentException(err_mess);
            //}
            this.arrIObligateQuantity = arrIObligateQuantity;
            this.chArrAlphabites = chArrAlphabites;
            int total_length = 0;
            for (int i = 0; i < chArrAlphabites.Length; i++)
            {
                total_length += chArrAlphabites[i].Length;
            }
            this.chArrAlphabite = new char[total_length];
            int idx = 0;
            foreach (char[] t in chArrAlphabites)
            {
                Array.Copy(t, 0, chArrAlphabite, idx, t.Length);
                idx += t.Length;
            }

            //this.iObligateQuantity = (uint) (iObligateQuantity % chArrObligateCharacters.Length);
            this.min_length = Math.Min(minLength, maxLength);
            this.max_length = Math.Max(minLength, maxLength);
            InitRandom();
        }

        /// <summary>
        /// Generates random password
        /// </summary>
        /// <returns></returns>
        public string GeneratePassword()
        {
            int pass_length = (min_length != max_length) ? GetRandom((int)min_length, (int)max_length) : (int)min_length;
            int alphabite_arr_pos = 0;
            int count = 0;
            bool bUnique = false;
            char[] chArrPass = new char[pass_length];
            IList<int> lPositions = new List<int>(pass_length);
            for (int i = 0; i < pass_length; i++)
            {
                lPositions.Add(i);
            }
            int pos;
            char ch;

            for (alphabite_arr_pos = 0; alphabite_arr_pos < chArrAlphabites.Length; alphabite_arr_pos++)
            {
                int countObligate = 0;
                bUnique = arrIObligateQuantity[alphabite_arr_pos] <= chArrAlphabites[alphabite_arr_pos].Length;
                while (countObligate++ < arrIObligateQuantity[alphabite_arr_pos])
                {
                    ch = chArrAlphabites[alphabite_arr_pos][GetRandom(0, chArrAlphabites[alphabite_arr_pos].Length)];
                    if (bUnique)
                    {
                        while (!IsUnique(chArrPass, ch))
                        {
                            ch = chArrAlphabites[alphabite_arr_pos][GetRandom(0, chArrAlphabites[alphabite_arr_pos].Length)];
                        }
                    }
                    pos = GetPosition(lPositions, pass_length);
                    chArrPass[pos] = ch;
                    count++;
                }
            }

            bUnique = pass_length <= chArrAlphabite.Length;
            for (int i = count; i < pass_length; i++)
            {
                pos = GetPosition(lPositions, pass_length);
                ch = chArrAlphabite[GetRandom(0, chArrAlphabite.Length)];
                if (bUnique)
                {
                    while (!IsUnique(chArrPass, ch))
                    {
                        ch = chArrAlphabite[GetRandom(0, chArrAlphabite.Length)];
                    }
                }
                chArrPass[pos] = ch;
            }
            return new string(chArrPass);
        }

        protected bool IsUnique(char[] chArr, char ch)
        {
            bool bRes = true;
            for (int i = 0; i < chArr.Length; i++)
            {
                if (ch.CompareTo(chArr[i]) == 0)
                {
                    bRes = false;
                    break;
                }
            }
            return bRes;
        }

        protected void InitRandom()
        {
            r = new Random();
        }

        protected int GetPosition(IList<int> lPositions, int length)
        {
            int idx, pos;
            idx = GetRandom(0, lPositions.Count);
            pos = lPositions[idx];
            lPositions.RemoveAt(idx);
            return pos;
        }

        protected int GetRandom(int min, int max)
        {
            return r.Next(min, max);
        }

        protected bool ValidateSequence(char[] chArr)
        {
            bool bValid = true;
            long boundary = chArr.Length;
            for (int i = 0; i < boundary && bValid; i++)
            {
                for (int j = i + 1; j < boundary; j++)
                {
                    if (chArr[i].CompareTo(chArr[j]) == 0)
                    {
                        bValid = false;
                        break;
                    }
                }
            }
            return bValid;
        }

    }
}
