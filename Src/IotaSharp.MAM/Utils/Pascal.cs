using System;

namespace IotaSharp.MAM.Utils
{
    public class Pascal
    {
        public static sbyte[] Zero = {1, 0, 0, -1};

        public static Tuple<long, int> Decode(sbyte[] trits)
        {
            // check Zero
            ulong index;
            for (index = 0; index < 4; index++)
            {
                if (trits[index] != Zero[index])
                    break;
            }

            if (index == 4)
                return new Tuple<long, int>(0, 4);

            //
            var encodersStart = End(trits, 0);
            var inputEnd = encodersStart +
                           PascalMinTrits((ulong) Math.Pow(2, (long) (encodersStart / Constants.TritsPerTryte)) - 1);

            var encoder = TritsHelper.Trits2Long(trits, (int) encodersStart, (int) (inputEnd - encodersStart));

            long acc = 0;
            int i = 0;
            index = 0;
            for (; index < encodersStart; index += Constants.TritsPerTryte, i++)
            {
                ulong left = encodersStart - index;
                int length = left > Constants.TritsPerTryte ? Constants.TritsPerTryte : (int) left;

                long temp = TritsHelper.Trits2Long(trits, (int) index, length);
                if (((encoder >> i) & 0x1L) != 0x0L)
                {
                    temp = -temp;
                }

                acc += (long) (Math.Pow(27, i)) * temp;

            }

            return new Tuple<long, int>(acc, (int) inputEnd);
        }

        public static int EncodedLength(long input)
        {
            if (input == 0)
                return Zero.Length;

            int length = (int) TritsHelper.RoundThird((long) PascalMinTrits((ulong) Math.Abs(input)));

            return length + (int) PascalMinTrits((ulong) Math.Pow(2, (uint) (length / Constants.TritsPerTryte)) - 1);
        }

        public static void Encode(long input, sbyte[] trits)
        {
            if (input == 0)
            {
                Array.Copy(Zero, trits, Zero.Length);
            }
            else
            {
                int length = (int) TritsHelper.RoundThird((long) PascalMinTrits((ulong) Math.Abs(input)));
                long encoding = 0;
                TritsHelper.Long2Trits(input, trits);
                int index = 0;

                int end = length - Constants.TritsPerTryte;
                for (int i = 0; i < end; i += Constants.TritsPerTryte)
                {
                    int n = end - i > Constants.TritsPerTryte ? Constants.TritsPerTryte : end - i;
                    if (TritsHelper.Trits2Long(trits, i, n) > 0)
                    {
                        encoding |= (1L << index);
                        for (int j = 0; j < n; j++)
                        {
                            trits[i + j] = (sbyte) -trits[i + j];
                        }
                    }

                    index += 1;
                }

                if (TritsHelper.Trits2Long(trits, length - Constants.TritsPerTryte, Constants.TritsPerTryte) < 0)
                {
                    encoding |= (1L << index);
                    for (int j = 0; j < Constants.TritsPerTryte; j++)
                    {
                        trits[length - Constants.TritsPerTryte + j] =
                            (sbyte) -trits[length - Constants.TritsPerTryte + j];
                    }
                }

                sbyte[] encodingTrits = new sbyte[trits.Length - length];
                TritsHelper.Long2Trits(encoding, encodingTrits);

                Array.Copy(encodingTrits, 0, trits, length, encodingTrits.Length);
            }
        }

        private static ulong End(sbyte[] trits, int index)
        {
            if (TritsHelper.Trits2Long(trits, index, Constants.TritsPerTryte) > 0)
            {
                return Constants.TritsPerTryte;
            }

            return Constants.TritsPerTryte + End(trits, index + Constants.TritsPerTryte);
        }

        private static ulong PascalMinTrits(ulong input)
        {
            return MinTritsHelper(input, 1);
        }

        private static ulong MinTritsHelper(ulong input, ulong baseNum)
        {
            if (input <= baseNum)
                return 1;

            return 1 + MinTritsHelper(input, 1 + baseNum * Constants.Radix);

        }
    }
}
