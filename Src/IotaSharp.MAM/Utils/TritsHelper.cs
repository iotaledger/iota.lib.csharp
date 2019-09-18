using System;

namespace IotaSharp.MAM.Utils
{
    public class TritsHelper
    {
        public static long Trits2Long(sbyte[] trits, int index, int length)
        {
            long value = 0;

            for (var i = index + length - 1; i >= index; i--)
            {
                value = value * 3 + trits[i];
            }

            return value;
        }

        public static ulong Long2Trits(long input, sbyte[] trits)
        {
            ulong end = WriteTrits(Math.Abs(input), trits);
            if (input < 0)
            {
                for (int i = 0; i < trits.Length; i++)
                {
                    trits[i] = (sbyte) -trits[i];
                }
            }

            return end;
        }

        private static ulong WriteTrits(long input, sbyte[] trits)
        {
            ulong index = 0;
            long abs = input;

            while (true)
            {
                if (abs == 0)
                    break;

                var r = (sbyte) (abs % Constants.Radix);
                abs = abs / Constants.Radix;

                if (r > 1)
                {
                    abs += 1;
                    r = -1;
                }

                trits[index] = r;
                index++;

            }

            return index;
        }

        public static long RoundThird(long input)
        {
            long rem = input % Constants.TritsPerTryte;

            if (rem == 0)
                return input;

            return input + Constants.TritsPerTryte - rem;

        }

        public static sbyte Sum(sbyte i, sbyte j)
        {
            sbyte s = (sbyte)(i + j);
            switch (s)
            {
                case -2:
                    return 1;
                case 2:
                    return -1;
                default:
                    return s;
            }
        }
    }
}
