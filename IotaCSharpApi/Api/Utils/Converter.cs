using System;
using System.Collections.Generic;
using System.Text;

namespace Iota.Lib.CSharp.Api.Utils
{
    public class Converter
    {
        public static readonly int Radix = 3;
        public static readonly int MaxTritValue = (Radix - 1)/2, MinTritValue = -MaxTritValue;

        public static readonly int NumberOfTritsInAByte = 5;
        public static readonly int NumberOfTritsInATryte = 3;

        static readonly int[][] ByteToTritsMappings = new int[243][];
        static readonly int[][] TryteToTritsMappings = new int[27][];

        public static readonly string TryteAlphabet = "9ABCDEFGHIJKLMNOPQRSTUVWXYZ";

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

        public static byte[] ToBytes(int[] trits, int offset, int size)
        {
            byte[] bytes = new byte[(size + NumberOfTritsInAByte - 1)/NumberOfTritsInAByte];
            for (int i = 0; i < bytes.Length; i++)
            {
                int value = 0;
                for (
                    int j = (size - i*NumberOfTritsInAByte) < 5
                        ? (size - i*NumberOfTritsInAByte)
                        : NumberOfTritsInAByte;
                    j-- > 0;)
                {
                    value = value*Radix + trits[offset + i*NumberOfTritsInAByte + j];
                }
                bytes[i] = (byte) value;
            }

            return bytes;
        }

        public static byte[] ToBytes(int[] trits)
        {
            return ToBytes(trits, 0, trits.Length);
        }

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

        public static int[] ToTritsString(string trytes)
        {
            int[] d = new int[3*trytes.Length];
            for (int i = 0; i < trytes.Length; i++)
            {
                Array.Copy(TryteToTritsMappings[Constants.TryteAlphabet.IndexOf(trytes[i])], 0, d,
                    i*NumberOfTritsInATryte, NumberOfTritsInATryte);
            }
            return d;
        }

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

        public static int[] ToTrits(string trytes)
        {
            List<int> trits = new List<int>();
            if (InputValidator.isValue(trytes))
            {
                long value = long.Parse(trytes);

                long absoluteValue = value < 0 ? -value : value;

                int position = 0;

                while (absoluteValue > 0)
                {
                    int remainder = (int) (absoluteValue%Radix);
                    absoluteValue /= Radix;

                    if (remainder > MaxTritValue)
                    {
                        remainder = MinTritValue;
                        absoluteValue++;
                    }

                    trits.Insert(position++, remainder);
                }
                if (value < 0)
                {
                    for (int i = 0; i < trits.Count; i++)
                    {
                        trits[i] = -trits[i];
                    }
                }
            }
            else
            {
                int[] d = new int[3*trytes.Length];
                for (int i = 0; i < trytes.Length; i++)
                {
                    Array.Copy(TryteToTritsMappings[Constants.TryteAlphabet.IndexOf(trytes[i])], 0, d,
                        i*NumberOfTritsInATryte, NumberOfTritsInATryte);
                }
                return d;
            }
            return trits.ToArray();
        }

        public static int[] CopyTrits(string input, int[] destination)
        {
            for (int i = 0; i < input.Length; i++)
            {
                int index = Constants.TryteAlphabet.IndexOf(input[i]);
                destination[i*3] = TryteToTritsMappings[index][0];
                destination[i*3 + 1] = TryteToTritsMappings[index][1];
                destination[i*3 + 2] = TryteToTritsMappings[index][2];
            }
            return destination;
        }

        public static void CopyTrits(long value, int[] destination, int offset, int size)
        {
            long absoluteValue = value < 0 ? -value : value;
            for (int i = 0; i < size; i++)
            {
                int remainder = (int) (absoluteValue%Radix);
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

        public static string ToTrytes(int[] trits, int offset, int size)
        {
            StringBuilder trytes = new StringBuilder();
            for (int i = 0; i < (size + NumberOfTritsInATryte - 1)/NumberOfTritsInATryte; i++)
            {
                int j = trits[offset + i*3] + trits[offset + i*3 + 1]*3 + trits[offset + i*3 + 2]*9;
                if (j < 0)
                {
                    j += TryteAlphabet.Length;
                }
                trytes.Append(TryteAlphabet[j]);
            }
            return trytes.ToString();
        }

        public static string ToTrytes(int[] trits)
        {
            return ToTrytes(trits, 0, trits.Length);
        }

        public static int ToTryteValue(int[] trits, int offset)
        {
            return trits[offset] + trits[offset + 1]*3 + trits[offset + 2]*9;
        }

        public static int ToValue(int[] trits)
        {
            int value = 0;

            for (int i = trits.Length; i-- > 0;)
            {
                value = value*3 + trits[i];
            }
            return value;
        }

        public static long ToLongValue(int[] trits)
        {
            long value = 0;

            for (int i = trits.Length; i-- > 0;)
            {
                value = value*3 + trits[i];
            }
            return value;
        }

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