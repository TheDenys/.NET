using System;
using System.Collections.Generic;
using System.Linq;

namespace Replacer.Instruments
{
    public class GarbleInstrument : Instrument
    {
        public string Instrument(string original)
        {
            string garbled = Garble(original, alphabet_subst);
            string instrument = string.Format("{0}{1}{2}", prefix, garbled, suffix);
            return instrument;
        }

        private const string prefix = "創", suffix = "末";
        private const string alphabet_subst = "zʎxʍʌnʇsɹbdouɯlʞɾıɥƃɟǝpɔqɐ";
        const string alphabet_orig = "abcdefghijklmnopqrstuvwxyz";
        private static List<char> alphabetOriginalList = alphabet_orig.ToCharArray().ToList();

        private static string Garble(string input, string filler)
        {
            if (input == null)
            {
                return input;
            }

            string alphabet_subst = filler ?? string.Empty;

            Func<char, int> GetPos = (ch) => alphabetOriginalList.IndexOf(char.ToLowerInvariant(ch));

            var reverted = input.ToCharArray().Reverse();
            var garbled = reverted.Select(
                ch =>
                {
                    var pos = GetPos(ch);
                    var pos_subst = (alphabet_subst.Length - 1) - pos;
                    switch (ch)
                    {
                        case '{':
                            return '}';
                        case '}':
                            return '{';
                        default:
                            return pos != -1 && pos_subst < alphabet_subst.Length ? alphabet_subst[(alphabet_subst.Length - 1) - pos] : ch;
                    }
                });

            string res = string.Join(null, garbled);
            return res;
        }
    }
}