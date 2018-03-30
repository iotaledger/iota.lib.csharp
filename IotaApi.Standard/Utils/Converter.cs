using System;
using System.Collections.Generic;
using System.Text;

namespace Iota.Api.Standard.Utils
{
    /// <summary>
    /// This class provides a set of utility methods to are used to convert between different formats
    /// </summary>
    public class Converter
    {
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// 
        /// </summary>
        public static readonly uint HIGH_INTEGER_BITS = 0xFFFFFFFF;

        /// <summary>
        /// 
        /// </summary>
        public static readonly ulong HIGH_LONG_BITS = 0xFFFFFFFFFFFFFFFFL;
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// The radix
        /// </summary>
        public static readonly int Radix = 3;

        /// <summary>
        /// The maximum trit value
        /// </summary>
        public static readonly int MaxTritValue = (Radix - 1) / 2;

        /// <summary>
        /// The minimum trit value
        /// </summary>
        public static readonly int MinTritValue = -MaxTritValue;

        /// <summary>
        /// The number of trits in a byte
        /// </summary>
        public static readonly int NumberOfTritsInAByte = 5;

        /// <summary>
        /// The number of trits in a tryte
        /// </summary>
        public static readonly int NumberOfTritsInATryte = 3;

        static readonly int[][] ByteToTritsMappings = new int[243][]; //why 243? max 121
        static readonly int[][] TryteToTritsMappings = new int[27][];

        static Converter()
        {
            int[] trits = new int[NumberOfTritsInAByte];

            for (int i = 0; i < 243; i++)
            {
                ByteToTritsMappings[i] = new int[NumberOfTritsInAByte];
                ByteToTritsMappings[i] = new int[NumberOfTritsInAByte];
                Array.Copy(trits, ByteToTritsMappings[i], NumberOfTritsInAByte);
                Increment(trits, NumberOfTritsInAByte);
            }

            for (int i = 0; i < 27; i++)
            {
                TryteToTritsMappings[i] = new int[NumberOfTritsInATryte];
                Array.Copy(trits, TryteToTritsMappings[i], NumberOfTritsInATryte);
                Increment(trits, NumberOfTritsInATryte);
            }
        }

        /// <summary>
        /// Converts the specified trits array to bytes
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static byte[] ToBytes(int[] trits, int offset, int size)
        {
            byte[] bytes = new byte[(size + NumberOfTritsInAByte - 1) / NumberOfTritsInAByte];
            for (int i = 0; i < bytes.Length; i++)
            {
                int value = 0;
                for (
                    int j = (size - i * NumberOfTritsInAByte) < 5
                        ? (size - i * NumberOfTritsInAByte)
                        : NumberOfTritsInAByte;
                    j-- > 0;)
                {
                    value = value * Radix + trits[offset + i * NumberOfTritsInAByte + j];
                }

                bytes[i] = (byte) value;
            }

            return bytes;
        }

        /// <summary>
        /// Converts the specified trits to trytes
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns></returns>
        public static byte[] ToBytes(int[] trits)
        {
            return ToBytes(trits, 0, trits.Length);
        }

        /// <summary>
        /// Gets the trits from the specified bytes and stores it into the provided trits array
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="trits">The trits.</param>
        public static void GetTrits(sbyte[] bytes, int[] trits)
        {
            int offset = 0;
            for (int i = 0; i < bytes.Length && offset < trits.Length; i++)
            {
                Array.Copy(
                    ByteToTritsMappings[bytes[i] < 0 ? (bytes[i] + ByteToTritsMappings.Length) : bytes[i]], 0,
                    trits, offset,
                    trits.Length - offset < NumberOfTritsInAByte
                        ? (trits.Length - offset)
                        : NumberOfTritsInAByte);

                offset += NumberOfTritsInAByte;
            }

            while (offset < trits.Length)
            {
                trits[offset++] = 0;
            }
        }

        /// <summary>
        /// Converts the specified trinary encoded string into a trits array of the specified length.
        /// If the trytes string results in a shorter then specified trits array, then the remainder is padded we zeroes
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <param name="length">The length.</param>
        /// <returns>a trits array</returns>
        public static int[] ToTrits(string trytes, int length)
        {
            int[] tritss = ToTrits(trytes);

            List<int> tritsList = new List<int>();

            foreach (int i in tritss)
                tritsList.Add(i);

            while (tritsList.Count < length)
                tritsList.Add(0);

            return tritsList.ToArray();
        }


        /// <summary>
        /// Converts the specified trinary encoded trytes string to trits
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <returns></returns>
        public static int[] ToTrits(string trytes)
        {
            int[] d = new int[3 * trytes.Length];
            for (int i = 0; i < trytes.Length; i++)
            {
                Array.Copy(TryteToTritsMappings[Constants.TryteAlphabet.IndexOf(trytes[i])], 0, d,
                    i * NumberOfTritsInATryte, NumberOfTritsInATryte);
            }

            return d;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trytes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int[] ToTrits(long trytes, int length)
        {
            int[] tritss = ToTrits(trytes);

            List<int> tritsList = new List<int>(tritss);

            while (tritsList.Count < length)
                tritsList.Add(0);

            return tritsList.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trytes"></param>
        /// <returns></returns>
        public static int[] ToTrits(long trytes)
        {
            if (trytes == 0)
                return new[] {0};

            var tritsList = new List<int>();
            
            while (trytes != 0)
            {
                var remainder = (int) (trytes % Radix);
                trytes /= Radix;

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
        /// Copies the trits from the input string into the destination array
        /// </summary>
        /// <param name="input">The input string</param>
        /// <param name="destination">The destination array.</param>
        /// <returns></returns>
        public static int[] CopyTrits(string input, int[] destination)
        {
            for (int i = 0; i < input.Length; i++)
            {
                int index = Constants.TryteAlphabet.IndexOf(input[i]);
                destination[i * 3] = TryteToTritsMappings[index][0];
                destination[i * 3 + 1] = TryteToTritsMappings[index][1];
                destination[i * 3 + 2] = TryteToTritsMappings[index][2];
            }

            return destination;
        }

        /// <summary>
        /// Copies the trits in long representation into the destination array
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="destination">The destination array.</param>
        /// <param name="offset">The offset from which copying is started.</param>
        /// <param name="size">The size.</param>
        public static void CopyTrits(long value, int[] destination, int offset, int size)
        {
            long absoluteValue = value < 0 ? -value : value;
            for (int i = 0; i < size; i++)
            {
                int remainder = (int) (absoluteValue % Radix);
                absoluteValue /= Radix;
                if (remainder > MaxTritValue)
                {
                    remainder = MinTritValue;
                    absoluteValue++;
                }

                destination[offset + i] = remainder;
            }

            if (value < 0)
            {
                for (int i = 0; i < size; i++)
                {
                    destination[offset + i] = -destination[offset + i];
                }
            }
        }

        /// <summary>
        /// Converts the trits array to a trytes string
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset from which copying is started.</param>
        /// <param name="size">The size.</param>
        /// <returns>a trytes string</returns>
        public static string ToTrytes(int[] trits, int offset, int size)
        {
            StringBuilder trytes = new StringBuilder();
            for (int i = 0; i < (size + NumberOfTritsInATryte - 1) / NumberOfTritsInATryte; i++)
            {
                int j = trits[offset + i * 3] + trits[offset + i * 3 + 1] * 3 + trits[offset + i * 3 + 2] * 9;
                if (j < 0)
                {
                    j += Constants.TryteAlphabet.Length;
                }

                trytes.Append(Constants.TryteAlphabet[j]);
            }

            return trytes.ToString();
        }

        /// <summary>
        /// Converts the trits array to a trytes string
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns>a trytes string</returns>
        public static string ToTrytes(int[] trits)
        {
            return ToTrytes(trits, 0, trits.Length);
        }

        /// <summary>
        /// Converts the specified trits array to trytes in integer representation
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>trytes in integer representation</returns>
        public static int ToTryteValue(int[] trits, int offset)
        {
            return trits[offset] + trits[offset + 1] * 3 + trits[offset + 2] * 9;
        }

        /// <summary>
        /// Converts the specified trits to its corresponding integer value
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns>an integer value representing the corresponding trits</returns>
        public static int ToValue(int[] trits)
        {
            int value = 0;

            for (int i = trits.Length; i-- > 0;)
            {
                value = value * 3 + trits[i];
            }

            return value;
        }

        /// <summary>
        ///  Converts the specified trits to its corresponding integer value
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns></returns>
        public static long ToLongValue(int[] trits)
        {
            long value = 0;

            for (int i = trits.Length; i-- > 0;)
            {
                value = value * 3 + trits[i];
            }

            return value;
        }

        /// <summary>
        /// Increments the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="size">The size.</param>
        public static void Increment(int[] trits, int size)
        {
            for (int i = 0; i < size; i++)
            {
                if (++trits[i] > MaxTritValue)
                {
                    trits[i] = MinTritValue;
                }
                else
                {
                    break;
                }
            }
        }
    }
}