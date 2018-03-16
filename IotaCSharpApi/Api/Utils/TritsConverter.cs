using System;
using System.Collections.Generic;
using System.Text;

namespace Iota.Lib.CSharp.Api.Utils
{
    /// <summary>
    /// </summary>
    public class TritsConverter
    {
        /// <summary>
        ///     The maximum trit value
        /// </summary>
        public const sbyte MaxTritValue = 1;

        /// <summary>
        ///     The minimum trit value
        /// </summary>
        public const sbyte MinTritValue = -1;

        /// <summary>
        ///     The number of trits in a byte
        /// </summary>
        public const int NumberOfTritsInAByte = 5;

        /// <summary>
        ///     The number of trits in a tryte
        /// </summary>
        public const int NumberOfTritsInATryte = 3;

        private static readonly sbyte[][] ByteToTritsMappings = new sbyte[243][];
        private static readonly sbyte[][] TryteToTritsMappings = new sbyte[27][];

        static TritsConverter()
        {
            var trits = new sbyte[NumberOfTritsInAByte];
            for (var i = 0; i < 243; i++)
            {
                ByteToTritsMappings[i] = new sbyte[NumberOfTritsInAByte];
                Array.Copy(trits, ByteToTritsMappings[i], NumberOfTritsInAByte);
                Increment(trits, NumberOfTritsInAByte);
            }

            trits = new sbyte[NumberOfTritsInATryte];
            for (var i = 0; i < 27; i++)
            {
                TryteToTritsMappings[i] = new sbyte[NumberOfTritsInATryte];
                Array.Copy(trits, TryteToTritsMappings[i], NumberOfTritsInATryte);
                Increment(trits, NumberOfTritsInATryte);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] ToBytes(sbyte[] trits, int offset, int size)
        {
            var byteList = new List<byte>();

            int index;
            var endIndex = offset + size - 5;
            int value;

            for (index = offset; index <= endIndex; index += 5)
            {
                value = trits[index] * 81
                        + trits[index + 1] * 27
                        + trits[index + 2] * 9
                        + trits[index + 3] * 3
                        + trits[index + 4];

                byteList.Add((byte) value);
            }

            // left
            endIndex = offset + size;
            if (index < endIndex)
            {
                value = 0;
                while (index < endIndex)
                {
                    value = value * 3 + trits[index];
                    index++;
                }

                byteList.Add((byte) value);
            }

            return byteList.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        public static byte[] ToBytes(sbyte[] trits)
        {
            return ToBytes(trits, 0, trits.Length);
        }

        /// <summary>
        /// </summary>
        /// <param name="trytes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static sbyte[] ToTrits(string trytes, int length)
        {
            var tritss = ToTrits(trytes);

            var tritsList = new List<sbyte>(tritss);

            while (tritsList.Count < length)
                tritsList.Add(0);

            return tritsList.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="trytes"></param>
        /// <returns></returns>
        public static sbyte[] ToTrits(string trytes)
        {
            var tritss = new sbyte[3 * trytes.Length];
            for (var i = 0; i < trytes.Length; i++)
                Array.Copy(TryteToTritsMappings[TrytesConverter.TryteToDecimal(trytes[i])], 0,
                    tritss, i * 3, 3);

            return tritss;
        }


        /// <summary>
        /// </summary>
        /// <param name="trytes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static sbyte[] ToTrits(long trytes, int length)
        {
            var tritss = ToTrits(trytes);

            var tritsList = new List<sbyte>(tritss);

            while (tritsList.Count < length)
                tritsList.Add(0);

            return tritsList.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="trytes"></param>
        /// <returns></returns>
        public static sbyte[] ToTrits(long trytes)
        {
            if (trytes == 0)
                return new sbyte[] {0};

            var tritsList = new List<sbyte>();

            while (trytes != 0)
            {
                var remainder = (sbyte) (trytes % 3);
                trytes /= 3;

                if (remainder > MaxTritValue)
                {
                    remainder = MinTritValue;
                    trytes += 1;
                }
                else if (remainder < MinTritValue)
                {
                    remainder = MaxTritValue;
                    trytes -= 1;
                }

                tritsList.Add(remainder);
            }

            return tritsList.ToArray();
        }


        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string ToTrytes(sbyte[] trits, int offset, int size)
        {
            var trytes = new StringBuilder();
            var endIndex = offset + size - 3;

            for (var i = offset; i <= endIndex; i += 3)
            {
                var j = trits[i] + trits[i + 1] * 3 + trits[i + 2] * 9;
                if (j < 0) j += Constants.TryteAlphabet.Length;

                trytes.Append(Constants.TryteAlphabet[j]);
            }

            if (size % 3 != 0)
            {
                //TODO(gjc): need handle???
            }


            return trytes.ToString();
        }

        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        public static string ToTrytes(sbyte[] trits)
        {
            return ToTrytes(trits, 0, trits.Length);
        }

        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        public static int ToInt32(sbyte[] trits)
        {
            var value = 0;

            for (var i = trits.Length - 1; i >= 0; i--) value = value * 3 + trits[i];

            return value;
        }

        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        public static long ToInt64(sbyte[] trits)
        {
            long value = 0;

            for (var i = trits.Length - 1; i >= 0; i--) value = value * 3 + trits[i];

            return value;
        }

        #region Private Method

        private static void Increment(sbyte[] trits, int size)
        {
            for (var i = 0; i < size; i++)
                if (++trits[i] > MaxTritValue)
                    trits[i] = MinTritValue;
                else
                    break;
        }

        #endregion
    }
}