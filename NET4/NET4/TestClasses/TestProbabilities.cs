using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NET4.TestClasses
{
    class TestProbabilities : RunableBase
    {
        [Run(0)]
        public void Monty_Hall_Simulation()
        {
            //var buf = GetNumbers(0, 2).Take(20).ToArray();

            //foreach (var b in buf)
            //{
            //    Debug(b + Environment.NewLine);
            //}

            BitArray bits = new BitArray(3);
            int[] emptyPosArr = new int[2];
            int[] posArr = new int[2];
            long total = 0;
            long[] counts = new long[5];
            int lastPos = counts.Length - 1;

            foreach (int win in GetNumbers(0, 2))
            {
                // reset array
                Array.Copy(emptyPosArr, posArr, 2);

                // get the user bet
                var bet = GetRandom(0, 2);

                // select what item to show

                // reset bits
                bits.SetAll(false);
                // set win number in bit array
                bits.Set(win, true);
                int show = -1;

                // user select win item and we have 2 options to show
                if (win == bet)
                {
                    // pick a random non-win option
                    var randomShow = GetRandom(0, 1);

                    var p1 = 0;
                    var p2 = 0;

                    // set non-win options into array
                    foreach (bool bit in bits)
                    {
                        if (!bit)
                        {
                            posArr[p1++] = p2;
                        }

                        p2++;
                    }

                    // pick a random one out of two
                    show = posArr[randomShow];
                }
                else
                {
                    // only 1 option to show here as the other one is winning
                    // set the bet
                    bits.Set(bet, true);
                    show = GetPosFromBits(bits, posArr);
                }

                // find a new bet position that excludes the shown non-win
                var newBet = -1;
                // reset bits
                bits.SetAll(false);
                // set old bet number in bit array
                bits.Set(bet, true);
                // set show number in bit array
                bits.Set(show, true);
                newBet = GetPosFromBits(bits, posArr);

                total++;
                counts[win]++;
                if (bet == win) counts[lastPos - 1]++;
                if (newBet == win) counts[lastPos]++;

                if (total % 100000 == 0)
                {
                    Debug($"total: {total} counts: [{string.Join(",", counts)}] %: [{string.Join(",", counts.Select(c => (100 * c) / total))}]");
                    Debug(Environment.NewLine);
                }
            }
        }

        private int GetPosFromBits(BitArray bits, int[] posArr)
        {
            bits.Not();
            // convert bits array into position to show
            bits.CopyTo(posArr, 0);

            switch (posArr[0])
            {
                case -7:
                    return 0;
                case -6:
                    return 1;
                case -4:
                    return 2;
                default:
                    return -1;
            }
        }

        private IEnumerable<int> GetNumbers(int min, int max)
        {
            for (; ; ) yield return PDNUtils.Help.RandomNumber.Next(min, max);
        }

        private int GetRandom(int min, int max) => PDNUtils.Help.RandomNumber.Next(min, max);

        [Run(1)]
        public void TestGen()
        {
            int lim = 3;
            long total = 0;
            long[] counts = new long[lim];

            foreach (int pos in GetNumbers(0, lim - 1))
            {
                total++;
                counts[pos]++;

                if (total % 4000000 == 0)
                {
                    Debug($"total: {total} counts: [{string.Join(",", counts)}] %: [{string.Join(",", counts.Select(c => (100 * c) / total))}]");
                    Debug(Environment.NewLine);
                }
            }
        }

        //private long GetShowNumber(BitArray bits, int[] posArr, int win, int bet)
        //{
        //    bits.SetAll(false);
        //    bits.Set(win, true);
        //    bits.Set(bet, true);
        //    return bits.CopyTo(posArr, 0);
        //}
    }
}
