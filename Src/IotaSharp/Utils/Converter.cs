using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace IotaSharp.Utils
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Converter
    {
        private const sbyte RADIX = 3;
        private const sbyte MAX_TRIT_VALUE = (RADIX - 1) / 2, MIN_TRIT_VALUE = -MAX_TRIT_VALUE;

        private const int NUMBER_OF_TRITS_IN_A_BYTE = 5;
        private const int NUMBER_OF_TRITS_IN_A_TRYTE = 3;
        private static readonly sbyte[][] BYTE_TO_TRITS_MAPPINGS = new sbyte[243][];
        private static readonly sbyte[][] TRYTE_TO_TRITS_MAPPINGS = new sbyte[27][];

        static Converter()
        {
            sbyte[] trits = new sbyte[NUMBER_OF_TRITS_IN_A_BYTE];

            for (int i = 0; i < 243; i++)
            {
                BYTE_TO_TRITS_MAPPINGS[i] = new sbyte[NUMBER_OF_TRITS_IN_A_BYTE];
                Array.Copy(trits, BYTE_TO_TRITS_MAPPINGS[i], NUMBER_OF_TRITS_IN_A_BYTE);
                Increment(trits, NUMBER_OF_TRITS_IN_A_BYTE);
            }

            for (int i = 0; i < 27; i++)
            {
                TRYTE_TO_TRITS_MAPPINGS[i] = new sbyte[NUMBER_OF_TRITS_IN_A_TRYTE];
                Array.Copy(trits, TRYTE_TO_TRITS_MAPPINGS[i], NUMBER_OF_TRITS_IN_A_TRYTE);
                Increment(trits, NUMBER_OF_TRITS_IN_A_TRYTE);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trytes"></param>
        /// <returns></returns>
        public static sbyte[] ToTrits(string trytes)
        {
            sbyte[] d = new sbyte[3 * trytes.Length];
            for (int i = 0; i < trytes.Length; i++)
            {
                Array.Copy(TRYTE_TO_TRITS_MAPPINGS[Constants.TRYTE_ALPHABET.IndexOf(trytes[i])], 0, d,
                    i * NUMBER_OF_TRITS_IN_A_TRYTE, NUMBER_OF_TRITS_IN_A_TRYTE);
            }

            return d;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trytes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static sbyte[] ToTrits(string trytes, int length)
        {
            var trits = ToTrits(trytes);
            sbyte[] result = new sbyte[length];

            int copyLength = trits.Length < length ? trits.Length : length;

            Array.Copy(trits, result, copyLength);

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static sbyte[] ToTrits(long value, int length)
        {
            var trits = ToTrits(value);
            sbyte[] result = new sbyte[length];

            int copyLength = trits.Length < length ? trits.Length : length;

            Array.Copy(trits, result, copyLength);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static sbyte[] ToTrits(long value)
        {
            if (value == 0)
                return new[] {(sbyte) 0};

            var tritsList = new List<sbyte>();

            while (value != 0)
            {
                var remainder = (sbyte) (value % RADIX);
                value /= RADIX;

                if (remainder > MAX_TRIT_VALUE)
                {
                    remainder = MIN_TRIT_VALUE;
                    value += 1;
                }
                else if (remainder < MIN_TRIT_VALUE)
                {
                    remainder = MAX_TRIT_VALUE;
                    value -= 1;
                }

                tritsList.Add(remainder);
            }

            return tritsList.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        public static string ToTrytes(sbyte[] trits)
        {
            return ToTrytes(trits, 0, trits.Length);
        }

        /// <summary>
        /// Converts the trits array to a trytes string
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset from which copying is started.</param>
        /// <param name="size">The size.</param>
        /// <returns>a trytes string</returns>
        public static string ToTrytes(sbyte[] trits, int offset, int size)
        {
            StringBuilder trytes = new StringBuilder();
            for (int i = 0; i < (size + NUMBER_OF_TRITS_IN_A_TRYTE - 1) / NUMBER_OF_TRITS_IN_A_TRYTE; i++)
            {
                int j = trits[offset + i * 3] + trits[offset + i * 3 + 1] * 3 + trits[offset + i * 3 + 2] * 9;
                if (j < 0)
                {
                    j += Constants.TRYTE_ALPHABET.Length;
                }

                trytes.Append(Constants.TRYTE_ALPHABET[j]);
            }

            return trytes.ToString();
        }

        /// <summary>
        /// Increments the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="size">The size.</param>
        public static void Increment(sbyte[] trits, int size)
        {
            for (int i = 0; i < size; i++)
            {
                if (++trits[i] > MAX_TRIT_VALUE)
                {
                    trits[i] = MIN_TRIT_VALUE;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        public static int ToInt32(sbyte[] trits)
        {
            int value = 0;

            for (int i = trits.Length; i-- > 0;)
            {
                value = value * 3 + trits[i];
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        public static long ToInt64(sbyte[] trits)
        {
            long value = 0;

            for (int i = trits.Length; i-- > 0;)
            {
                value = value * 3 + trits[i];
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static long ToInt64(sbyte[] trits, int index, int length)
        {
            long value = 0;

            for (int i = index + length - 1; i >= index; i--)
            {
                value = value * 3 + trits[i];
            }

            return value;
        }
    }
}
