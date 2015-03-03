﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P089_RomanNumerals : RunableBase
    {
        enum R
        {
            I = 1,
            V = 5,
            X = 10,
            L = 50,
            C = 100,
            D = 500,
            M = 1000,
        }

        [Run(1)]
        public void SolveIt()
        {
            /*var roman = ToRoman(19);
            roman = ToRoman(357);
            roman = ToRoman(1357);
            roman = ToRoman(2452);
            roman = ToRoman(948);
            var seq = GetRomanSeq("MDCLIXVIII").ToList();
            FromRoman(seq);
            long n = FromRoman(new[] { R.M, R.D, R.C, R.C, R.L, R.X, R.V, R.I });
            */

            StringReader sr = new StringReader(data);
            string line;
            StringBuilder sbRes = new StringBuilder();
            int saved = 0;
            while ((line = sr.ReadLine()) != null)
            {
                var romanTrim = line.Trim();
                var r = FromRoman(GetRomanSeq(romanTrim).ToList());
                var newR = ToRoman(r);
                var diff = romanTrim.Length - newR.Length;
                saved += diff;
                sbRes.AppendLine(newR);
            }

            DebugFormat("oldLength={0} newLength={1} diff={2} saved={3}", data.Length, sbRes.Length, data.Length - sbRes.Length, saved);
        }

        static long FromRoman(IList<R> romanNumbers)
        {
            long val = 0;
            R current, next;
            int length = romanNumbers.Count;
            int pos = 0;

            foreach (var r in romanNumbers)
            {
                current = next = r;
                if (pos++ < length - 1)
                    next = romanNumbers[pos];
                int mul = ((int)current >= (int)next) ? 1 : -1;
                val += mul * (int)current;
            }

            return val;
        }

        static string ToRoman(long n)
        {
            List<R> roman = new List<R>(10);

            long buf = n;
            int mul;

            if (buf >= 1000)
            {
                mul = (int)(buf / (int)R.M);
                buf -= (int)R.M * mul;
                roman.AddRange(Enumerable.Repeat(R.M, mul));
            }

            if (buf >= 900)
            {
                buf -= 900;
                roman.Add(R.C);
                roman.Add(R.M);
            }

            if (900 > buf && buf >= 500)
            {
                roman.Add(R.D);
                buf -= 500;
                mul = (int)(buf / (int)R.C);
                buf -= (int)R.C * mul;
                roman.AddRange(Enumerable.Repeat(R.C, mul));
            }

            if (buf >= 400)
            {
                buf -= 400;
                roman.Add(R.C);
                roman.Add(R.D);
            }

            if (buf >= 100)
            {
                mul = (int)(buf / (int)R.C);
                buf -= (int)R.C * mul;
                roman.AddRange(Enumerable.Repeat(R.C, mul));
            }

            if (buf >= 90)
            {
                buf -= 90;
                roman.Add(R.C);
                roman.Add(R.M);
            }

            if (90 > buf && buf >= 50)
            {
                roman.Add(R.L);
                buf -= 50;
                mul = (int)(buf / (int)R.X);
                buf -= (int)R.X * mul;
                roman.AddRange(Enumerable.Repeat(R.X, mul));
            }

            if (buf >= 40)
            {
                buf -= 40;
                roman.Add(R.X);
                roman.Add(R.L);
            }

            if (buf >= 10)
            {
                mul = (int)(buf / (int)R.X);
                buf -= (int)R.X * mul;
                roman.AddRange(Enumerable.Repeat(R.X, mul));
            }

            if (buf == 9)
            {
                buf -= 9;
                roman.Add(R.I);
                roman.Add(R.X);
            }

            if (9 > buf && buf >= 5)
            {
                roman.Add(R.V);
                buf -= 5;
                mul = (int)(buf / (int)R.I);
                buf -= (int)R.I * mul;
                roman.AddRange(Enumerable.Repeat(R.I, mul));
            }

            if (buf == 4)
            {
                buf -= 4;
                roman.Add(R.I);
                roman.Add(R.V);
            }

            if (buf >= 1)
            {
                mul = (int)(buf / (int)R.I);
                buf -= (int)R.I * mul;
                roman.AddRange(Enumerable.Repeat(R.I, mul));
            }

            if (buf != 0)
                throw new Exception("wrong " + buf);

            return roman.Select(r => r.ToString()).Aggregate((e1, e2) => e1 + e2);
        }

        static IEnumerable<R> GetRomanSeq(string roman)
        {
            R r;
            foreach (var c in roman.ToUpperInvariant())
                if (Enum.TryParse<R>(c.ToString(), true, out r))
                    yield return r;
        }

        private const string data =
@"MMMMDCLXXII
MMDCCCLXXXIII
MMMDLXVIIII
MMMMDXCV
DCCCLXXII
MMCCCVI
MMMCDLXXXVII
MMMMCCXXI
MMMCCXX
MMMMDCCCLXXIII
MMMCCXXXVII
MMCCCLXXXXIX
MDCCCXXIIII
MMCXCVI
CCXCVIII
MMMCCCXXXII
MDCCXXX
MMMDCCCL
MMMMCCLXXXVI
MMDCCCXCVI
MMMDCII
MMMCCXII
MMMMDCCCCI
MMDCCCXCII
MDCXX
CMLXXXVII
MMMXXI
MMMMCCCXIV
MLXXII
MCCLXXVIIII
MMMMCCXXXXI
MMDCCCLXXII
MMMMXXXI
MMMDCCLXXX
MMDCCCLXXIX
MMMMLXXXV
MCXXI
MDCCCXXXVII
MMCCCLXVII
MCDXXXV
CCXXXIII
CMXX
MMMCLXIV
MCCCLXXXVI
DCCCXCVIII
MMMDCCCCXXXIV
CDXVIIII
MMCCXXXV
MDCCCXXXII
MMMMD
MMDCCLXIX
MMMMCCCLXXXXVI
MMDCCXLII
MMMDCCCVIIII
DCCLXXXIIII
MDCCCCXXXII
MMCXXVII
DCCCXXX
CCLXIX
MMMXI
MMMMCMLXXXXVIII
MMMMDLXXXVII
MMMMDCCCLX
MMCCLIV
CMIX
MMDCCCLXXXIIII
CLXXXII
MMCCCCXXXXV
MMMMDLXXXVIIII
MMMDCCCXXI
MMDCCCCLXXVI
MCCCCLXX
MMCDLVIIII
MMMDCCCLIX
MMMMCCCCXIX
MMMDCCCLXXV
XXXI
CDLXXXIII
MMMCXV
MMDCCLXIII
MMDXXX
MMMMCCCLVII
MMMDCI
MMMMCDLXXXIIII
MMMMCCCXVI
CCCLXXXVIII
MMMMCML
MMMMXXIV
MMMCCCCXXX
DCCX
MMMCCLX
MMDXXXIII
CCCLXIII
MMDCCXIII
MMMCCCXLIV
CLXXXXI
CXVI
MMMMCXXXIII
CLXX
DCCCXVIII
MLXVII
DLXXXX
MMDXXI
MMMMDLXXXXVIII
MXXII
LXI
DCCCCXLIII
MMMMDV
MMMMXXXIV
MDCCCLVIII
MMMCCLXXII
MMMMDCCXXXVI
MMMMLXXXIX
MDCCCLXXXI
MMMMDCCCXV
MMMMCCCCXI
MMMMCCCLIII
MDCCCLXXI
MMCCCCXI
MLXV
MMCDLXII
MMMMDXXXXII
MMMMDCCCXL
MMMMCMLVI
CCLXXXIV
MMMDCCLXXXVI
MMCLII
MMMCCCCXV
MMLXXXIII
MMMV
MMMV
DCCLXII
MMDCCCCXVI
MMDCXLVIII
CCLIIII
CCCXXV
MMDCCLXXXVIIII
MMMMDCLXXVIII
MMMMDCCCXCI
MMMMCCCXX
MMCCXLV
MMMDCCCLXIX
MMCCLXIIII
MMMDCCCXLIX
MMMMCCCLXIX
CMLXXXXI
MCMLXXXIX
MMCDLXI
MMDCLXXVIII
MMMMDCCLXI
MCDXXV
DL
CCCLXXII
MXVIIII
MCCCCLXVIII
CIII
MMMDCCLXXIIII
MMMDVIII
MMMMCCCLXXXXVII
MMDXXVII
MMDCCLXXXXV
MMMMCXLVI
MMMDCCLXXXII
MMMDXXXVI
MCXXII
CLI
DCLXXXIX
MMMCLI
MDCLXIII
MMMMDCCXCVII
MMCCCLXXXV
MMMDCXXVIII
MMMCDLX
MMMCMLII
MMMIV
MMMMDCCCLVIII
MMMDLXXXVIII
MCXXIV
MMMMLXXVI
CLXXIX
MMMCCCCXXVIIII
DCCLXXXV
MMMDCCCVI
LI
CLXXXVI
MMMMCCCLXXVI
MCCCLXVI
CCXXXIX
MMDXXXXI
MMDCCCXLI
DCCCLXXXVIII
MMMMDCCCIV
MDCCCCXV
MMCMVI
MMMMCMLXXXXV
MMDCCLVI
MMMMCCXLVIII
DCCCCIIII
MMCCCCIII
MMMDCCLXXXVIIII
MDCCCLXXXXV
DVII
MMMV
DCXXV
MMDCCCXCV
DCVIII
MMCDLXVI
MCXXVIII
MDCCXCVIII
MMDCLX
MMMDCCLXIV
MMCDLXXVII
MMDLXXXIIII
MMMMCCCXXII
MMMDCCCXLIIII
DCCCCLXVII
MMMCLXXXXIII
MCCXV
MMMMDCXI
MMMMDCLXXXXV
MMMCCCLII
MMCMIX
MMDCCXXV
MMDLXXXVI
MMMMDCXXVIIII
DCCCCXXXVIIII
MMCCXXXIIII
MMDCCLXXVIII
MDCCLXVIIII
MMCCLXXXV
MMMMDCCCLXXXVIII
MMCMXCI
MDXLII
MMMMDCCXIV
MMMMLI
DXXXXIII
MMDCCXI
MMMMCCLXXXIII
MMMDCCCLXXIII
MDCLVII
MMCD
MCCCXXVII
MMMMDCCIIII
MMMDCCXLVI
MMMCLXXXVII
MMMCCVIIII
MCCCCLXXIX
DL
DCCCLXXVI
MMDXCI
MMMMDCCCCXXXVI
MMCII
MMMDCCCXXXXV
MMMCDXLV
MMDCXXXXIV
MMD
MDCCCLXXXX
MMDCXLIII
MMCCXXXII
MMDCXXXXVIIII
DCCCLXXI
MDXCVIIII
MMMMCCLXXVIII
MDCLVIIII
MMMCCCLXXXIX
MDCLXXXV
MDLVIII
MMMMCCVII
MMMMDCXIV
MMMCCCLXIIII
MMIIII
MMMMCCCLXXIII
CCIII
MMMCCLV
MMMDXIII
MMMCCCXC
MMMDCCCXXI
MMMMCCCCXXXII
CCCLVI
MMMCCCLXXXVI
MXVIIII
MMMCCCCXIIII
CLXVII
MMMCCLXX
CCCCLXIV
MMXXXXII
MMMMCCLXXXX
MXL
CCXVI
CCCCLVIIII
MMCCCII
MCCCLVIII
MMMMCCCX
MCDLXXXXIV
MDCCCXIII
MMDCCCXL
MMMMCCCXXIII
DXXXIV
CVI
MMMMDCLXXX
DCCCVII
MMCMLXIIII
MMMDCCCXXXIII
DCCC
MDIII
MMCCCLXVI
MMMCCCCLXXI
MMDCCCCXVIII
CCXXXVII
CCCXXV
MDCCCXII
MMMCMV
MMMMCMXV
MMMMDCXCI
DXXI
MMCCXLVIIII
MMMMCMLII
MDLXXX
MMDCLXVI
CXXI
MMMDCCCLIIII
MMMCXXI
MCCIII
MMDCXXXXI
CCXCII
MMMMDXXXV
MMMCCCLXV
MMMMDLXV
MMMCCCCXXXII
MMMCCCVIII
DCCCCLXXXXII
MMCLXIV
MMMMCXI
MLXXXXVII
MMMCDXXXVIII
MDXXII
MLV
MMMMDLXVI
MMMCXII
XXXIII
MMMMDCCCXXVI
MMMLXVIIII
MMMLX
MMMCDLXVII
MDCCCLVII
MMCXXXVII
MDCCCCXXX
MMDCCCLXIII
MMMMDCXLIX
MMMMCMXLVIII
DCCCLXXVIIII
MDCCCLIII
MMMCMLXI
MMMMCCLXI
MMDCCCLIII
MMMDCCCVI
MMDXXXXIX
MMCLXXXXV
MMDXXX
MMMXIII
DCLXXIX
DCCLXII
MMMMDCCLXVIII
MDCCXXXXIII
CCXXXII
MMMMDCXXV
MMMCCCXXVIII
MDCVIII
MMMCLXXXXIIII
CLXXXI
MDCCCCXXXIII
MMMMDCXXX
MMMDCXXIV
MMMCCXXXVII
MCCCXXXXIIII
CXVIII
MMDCCCCIV
MMMMCDLXXV
MMMDLXIV
MDXCIII
MCCLXXXI
MMMDCCCXXIV
MCXLIII
MMMDCCCI
MCCLXXX
CCXV
MMDCCLXXI
MMDLXXXIII
MMMMDCXVII
MMMCMLXV
MCLXVIII
MMMMCCLXXVI
MMMDCCLXVIIII
MMMMDCCCIX
DLXXXXIX
DCCCXXII
MMMMIII
MMMMCCCLXXVI
DCCCXCIII
DXXXI
MXXXIIII
CCXII
MMMDCCLXXXIIII
MMMCXX
MMMCMXXVII
DCCCXXXX
MMCDXXXVIIII
MMMMDCCXVIII
LV
MMMDCCCCVI
MCCCII
MMCMLXVIIII
MDCCXI
MMMMDLXVII
MMCCCCLXI
MMDCCV
MMMCCCXXXIIII
MMMMDI
MMMDCCCXCV
MMDCCLXXXXI
MMMDXXVI
MMMDCCCLVI
MMDCXXX
MCCCVII
MMMMCCCLXII
MMMMXXV
MMCMXXV
MMLVI
MMDXXX
MMMMCVII
MDC
MCCIII
MMMMDCC
MMCCLXXV
MMDCCCXXXXVI
MMMMCCCLXV
CDXIIII
MLXIIII
CCV
MMMCMXXXI
CCCCLXVI
MDXXXII
MMMMCCCLVIII
MMV
MMMCLII
MCMLI
MMDCCXX
MMMMCCCCXXXVI
MCCLXXXI
MMMCMVI
DCCXXX
MMMMCCCLXV
DCCCXI
MMMMDCCCXIV
CCCXXI
MMDLXXV
CCCCLXXXX
MCCCLXXXXII
MMDCIX
DCCXLIIII
DXIV
MMMMCLII
CDLXI
MMMCXXVII
MMMMDCCCCLXIII
MMMDCLIIII
MCCCCXXXXII
MMCCCLX
CCCCLIII
MDCCLXXVI
MCMXXIII
MMMMDLXXVIII
MMDCCCCLX
MMMCCCLXXXX
MMMCDXXVI
MMMDLVIII
CCCLXI
MMMMDCXXII
MMDCCCXXI
MMDCCXIII
MMMMCLXXXVI
MDCCCCXXVI
MDV
MMDCCCCLXXVI
MMMMCCXXXVII
MMMDCCLXXVIIII
MMMCCCCLXVII
DCCXLI
MMCLXXXVIII
MCCXXXVI
MMDCXLVIII
MMMMCXXXII
MMMMDCCLXVI
MMMMCMLI
MMMMCLXV
MMMMDCCCXCIV
MCCLXXVII
LXXVIIII
DCCLII
MMMCCCXCVI
MMMCLV
MMDCCCXXXXVIII
DCCCXV
MXC
MMDCCLXXXXVII
MMMMCML
MMDCCCLXXVIII
DXXI
MCCCXLI
DCLXXXXI
MMCCCLXXXXVIII
MDCCCCLXXVIII
MMMMDXXV
MMMDCXXXVI
MMMCMXCVII
MMXVIIII
MMMDCCLXXIV
MMMCXXV
DXXXVIII
MMMMCLXVI
MDXII
MMCCCLXX
CCLXXI
DXIV
MMMCLIII
DLII
MMMCCCXLIX
MMCCCCXXVI
MMDCXLIII
MXXXXII
CCCLXXXV
MDCLXXVI
MDCXII
MMMCCCLXXXIII
MMDCCCCLXXXII
MMMMCCCLXXXV
MMDCXXI
DCCCXXX
MMMDCCCCLII
MMMDCCXXII
MMMMCDXCVIII
MMMCCLXVIIII
MMXXV
MMMMCDXIX
MMMMCCCX
MMMCCCCLXVI
MMMMDCLXXVIIII
MMMMDCXXXXIV
MMMCMXII
MMMMXXXIII
MMMMDLXXXII
DCCCLIV
MDXVIIII
MMMCLXXXXV
CCCCXX
MMDIX
MMCMLXXXVIII
DCCXLIII
DCCLX
D
MCCCVII
MMMMCCCLXXXIII
MDCCCLXXIIII
MMMDCCCCLXXXVII
MMMMCCCVII
MMMDCCLXXXXVI
CDXXXIV
MCCLXVIII
MMMMDLX
MMMMDXII
MMMMCCCCLIIII
MCMLXXXXIII
MMMMDCCCIII
MMDCLXXXIII
MDCCCXXXXIV
XXXXVII
MMMDCCCXXXII
MMMDCCCXLII
MCXXXV
MDCXXVIIII
MMMCXXXXIIII
MMMMCDXVII
MMMDXXIII
MMMMCCCCLXI
DCLXXXXVIIII
LXXXXI
CXXXIII
MCDX
MCCLVII
MDCXXXXII
MMMCXXIV
MMMMLXXXX
MMDCCCCXLV
MLXXX
MMDCCCCLX
MCDLIII
MMMCCCLXVII
MMMMCCCLXXIV
MMMDCVIII
DCCCCXXIII
MMXCI
MMDCCIV
MMMMDCCCXXXIV
CCCLXXI
MCCLXXXII
MCMIII
CCXXXI
DCCXXXVIII
MMMMDCCXLVIIII
MMMMCMXXXV
DCCCLXXV
DCCXCI
MMMMDVII
MMMMDCCCLXVIIII
CCCXCV
MMMMDCCXX
MCCCCII
MMMCCCXC
MMMCCCII
MMDCCLXXVII
MMDCLIIII
CCXLIII
MMMDCXVIII
MMMCCCIX
MCXV
MMCCXXV
MLXXIIII
MDCCXXVI
MMMCCCXX
MMDLXX
MMCCCCVI
MMDCCXX
MMMMDCCCCXCV
MDCCCXXXII
MMMMDCCCCXXXX
XCIV
MMCCCCLX
MMXVII
MLXXI
MMMDXXVIII
MDCCCCII
MMMCMLVII
MMCLXXXXVIII
MDCCCCLV
MCCCCLXXIIII
MCCCLII
MCDXLVI
MMMMDXVIII
DCCLXXXIX
MMMDCCLXIV
MDCCCCXLIII
CLXXXXV
MMMMCCXXXVI
MMMDCCCXXI
MMMMCDLXXVII
MCDLIII
MMCCXLVI
DCCCLV
MCDLXX
DCLXXVIII
MMDCXXXIX
MMMMDCLX
MMDCCLI
MMCXXXV
MMMCCXII
MMMMCMLXII
MMMMCCV
MCCCCLXIX
MMMMCCIII
CLXVII
MCCCLXXXXIIII
MMMMDCVIII
MMDCCCLXI
MMLXXIX
CMLXIX
MMDCCCXLVIIII
DCLXII
MMMCCCXLVII
MDCCCXXXV
MMMMDCCXCVI
DCXXX
XXVI
MMLXIX
MMCXI
DCXXXVII
MMMMCCCXXXXVIII
MMMMDCLXI
MMMMDCLXXIIII
MMMMVIII
MMMMDCCCLXII
MDCXCI
MMCCCXXIIII
CCCCXXXXV
MMDCCCXXI
MCVI
MMDCCLXVIII
MMMMCXL
MLXVIII
CMXXVII
CCCLV
MDCCLXXXIX
MMMCCCCLXV
MMDCCLXII
MDLXVI
MMMCCCXVIII
MMMMCCLXXXI
MMCXXVII
MMDCCCLXVIII
MMMCXCII
MMMMDCLVIII
MMMMDCCCXXXXII
MMDCCCCLXXXXVI
MDCCXL
MDCCLVII
MMMMDCCCLXXXVI
DCCXXXIII
MMMMDCCCCLXXXV
MMCCXXXXVIII
MMMCCLXXVIII
MMMDCLXXVIII
DCCCI
MMMMLXXXXVIIII
MMMCCCCLXXII
MMCLXXXVII
CCLXVI
MCDXLIII
MMCXXVIII
MDXIV
CCCXCVIII
CLXXVIII
MMCXXXXVIIII
MMMDCLXXXIV
CMLVIII
MCDLIX
MMMMDCCCXXXII
MMMMDCXXXIIII
MDCXXI
MMMDCXLV
MCLXXVIII
MCDXXII
IV
MCDLXXXXIII
MMMMDCCLXV
CCLI
MMMMDCCCXXXVIII
DCLXII
MCCCLXVII
MMMMDCCCXXXVI
MMDCCXLI
MLXI
MMMCDLXVIII
MCCCCXCIII
XXXIII
MMMDCLXIII
MMMMDCL
DCCCXXXXIIII
MMDLVII
DXXXVII
MCCCCXXIIII
MCVII
MMMMDCCXL
MMMMCXXXXIIII
MCCCCXXIV
MMCLXVIII
MMXCIII
MDCCLXXX
MCCCLIIII
MMDCLXXI
MXI
MCMLIV
MMMCCIIII
DCCLXXXVIIII
MDCLIV
MMMDCXIX
CMLXXXI
DCCLXXXVII
XXV
MMMXXXVI
MDVIIII
CLXIII
MMMCDLVIIII
MMCCCCVII
MMMLXX
MXXXXII
MMMMCCCLXVIII
MMDCCCXXVIII
MMMMDCXXXXI
MMMMDCCCXXXXV
MMMXV
MMMMCCXVIIII
MMDCCXIIII
MMMXXVII
MDCCLVIIII
MMCXXIIII
MCCCLXXIV
DCLVIII
MMMLVII
MMMCXLV
MMXCVII
MMMCCCLXXXVII
MMMMCCXXII
DXII
MMMDLV
MCCCLXXVIII
MMMCLIIII
MMMMCLXXXX
MMMCLXXXIIII
MDCXXIII
MMMMCCXVI
MMMMDLXXXIII
MMMDXXXXIII
MMMMCCCCLV
MMMDLXXXI
MMMCCLXXVI
MMMMXX
MMMMDLVI
MCCCCLXXX
MMMXXII
MMXXII
MMDCCCCXXXI
MMMDXXV
MMMDCLXXXVIIII
MMMDLXXXXVII
MDLXIIII
CMXC
MMMXXXVIII
MDLXXXVIII
MCCCLXXVI
MMCDLIX
MMDCCCXVIII
MDCCCXXXXVI
MMMMCMIV
MMMMDCIIII
MMCCXXXV
XXXXVI
MMMMCCXVII
MMCCXXIV
MCMLVIIII
MLXXXIX
MMMMLXXXIX
CLXXXXIX
MMMDCCCCLVIII
MMMMCCLXXIII
MCCCC
DCCCLIX
MMMCCCLXXXII
MMMCCLXVIIII
MCLXXXV
CDLXXXVII
DCVI
MMX
MMCCXIII
MMMMDCXX
MMMMXXVIII
DCCCLXII
MMMMCCCXLIII
MMMMCLXV
DXCI
MMMMCLXXX
MMMDCCXXXXI
MMMMXXXXVI
DCLX
MMMCCCXI
MCCLXXX
MMCDLXXII
DCCLXXI
MMMCCCXXXVI
MCCCCLXXXVIIII
CDLVIII
DCCLVI
MMMMDCXXXVIII
MMCCCLXXXIII
MMMMDCCLXXV
MMMXXXVI
CCCLXXXXIX
CV
CCCCXIII
CCCCXVI
MDCCCLXXXIIII
MMDCCLXXXII
MMMMCCCCLXXXI
MXXV
MMCCCLXXVIIII
MMMCCXII
MMMMCCXXXIII
MMCCCLXXXVI
MMMDCCCLVIIII
MCCXXXVII
MDCLXXV
XXXV
MMDLI
MMMCCXXX
MMMMCXXXXV
CCCCLIX
MMMMDCCCLXXIII
MMCCCXVII
DCCCXVI
MMMCCCXXXXV
MDCCCCXCV
CLXXXI
MMMMDCCLXX
MMMDCCCIII
MMCLXXVII
MMMDCCXXIX
MMDCCCXCIIII
MMMCDXXIIII
MMMMXXVIII
MMMMDCCCCLXVIII
MDCCCXX
MMMMCDXXI
MMMMDLXXXIX
CCXVI
MDVIII
MMCCLXXI
MMMDCCCLXXI
MMMCCCLXXVI
MMCCLXI
MMMMDCCCXXXIV
DLXXXVI
MMMMDXXXII
MMMXXIIII
MMMMCDIV
MMMMCCCXLVIII
MMMMCXXXVIII
MMMCCCLXVI
MDCCXVIII
MMCXX
CCCLIX
MMMMDCCLXXII
MDCCCLXXV
MMMMDCCCXXIV
DCCCXXXXVIII
MMMDCCCCXXXVIIII
MMMMCCXXXV
MDCLXXXIII
MMCCLXXXIV
MCLXXXXIIII
DXXXXIII
MCCCXXXXVIII
MMCLXXIX
MMMMCCLXIV
MXXII
MMMCXIX
MDCXXXVII
MMDCCVI
MCLXXXXVIII
MMMCXVI
MCCCLX
MMMCDX
CCLXVIIII
MMMCCLX
MCXXVIII
LXXXII
MCCCCLXXXI
MMMI
MMMCCCLXIV
MMMCCCXXVIIII
CXXXVIII
MMCCCXX
MMMCCXXVIIII
MCCLXVI
MMMCCCCXXXXVI
MMDCCXCIX
MCMLXXI
MMCCLXVIII
CDLXXXXIII
MMMMDCCXXII
MMMMDCCLXXXVII
MMMDCCLIV
MMCCLXIII
MDXXXVII
DCCXXXIIII
MCII
MMMDCCCLXXI
MMMLXXIII
MDCCCLIII
MMXXXVIII
MDCCXVIIII
MDCCCCXXXVII
MMCCCXVI
MCMXXII
MMMCCCLVIII
MMMMDCCCXX
MCXXIII
MMMDLXI
MMMMDXXII
MDCCCX
MMDXCVIIII
MMMDCCCCVIII
MMMMDCCCCXXXXVI
MMDCCCXXXV
MMCXCIV
MCMLXXXXIII
MMMCCCLXXVI
MMMMDCLXXXV
CMLXIX
DCXCII
MMXXVIII
MMMMCCCXXX
XXXXVIIII";
    }
}