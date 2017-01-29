using System;
using System.Collections.Generic;
using System.Text;
using Iota.Lib.CSharp.Api.Model;

namespace Iota.Lib.CSharp.Api.Utils
{
    public class Converter
    {
        public static readonly int RADIX = 3;
        public static readonly int MAX_TRIT_VALUE = (RADIX - 1)/2, MIN_TRIT_VALUE = -MAX_TRIT_VALUE;

        public static readonly int NUMBER_OF_TRITS_IN_A_BYTE = 5;
        public static readonly int NUMBER_OF_TRITS_IN_A_TRYTE = 3;

        static int[][] BYTE_TO_TRITS_MAPPINGS = new int[243][];
        static int[][] TRYTE_TO_TRITS_MAPPINGS = new int[27][];

        public static readonly string TRYTE_ALPHABET = "9ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        static Converter()
        {
            int[] trits = new int[NUMBER_OF_TRITS_IN_A_BYTE];

            for (int i = 0; i < 243; i++)
            {
                BYTE_TO_TRITS_MAPPINGS[i] = new int[NUMBER_OF_TRITS_IN_A_BYTE];
                // BYTE_TO_TRITS_MAPPINGS[i] = Arrays.copyOf(trits, NUMBER_OF_TRITS_IN_A_BYTE);
                BYTE_TO_TRITS_MAPPINGS[i] = new int[NUMBER_OF_TRITS_IN_A_BYTE];
                Array.Copy(trits, BYTE_TO_TRITS_MAPPINGS[i], NUMBER_OF_TRITS_IN_A_BYTE);
                increment(trits, NUMBER_OF_TRITS_IN_A_BYTE);
            }

            for (int i = 0; i < 27; i++)
            {
                //TRYTE_TO_TRITS_MAPPINGS[i] = Arrays.copyOf(trits, NUMBER_OF_TRITS_IN_A_TRYTE);
                TRYTE_TO_TRITS_MAPPINGS[i] = new int[NUMBER_OF_TRITS_IN_A_TRYTE];
                Array.Copy(trits, TRYTE_TO_TRITS_MAPPINGS[i], NUMBER_OF_TRITS_IN_A_TRYTE);
                increment(trits, NUMBER_OF_TRITS_IN_A_TRYTE);
            }
        }

        public static byte[] bytes(int[] trits, int offset, int size)
        {
            byte[] bytes = new byte[(size + NUMBER_OF_TRITS_IN_A_BYTE - 1)/NUMBER_OF_TRITS_IN_A_BYTE];
            for (int i = 0; i < bytes.Length; i++)
            {
                int value = 0;
                for (
                    int j = (size - i*NUMBER_OF_TRITS_IN_A_BYTE) < 5
                        ? (size - i*NUMBER_OF_TRITS_IN_A_BYTE)
                        : NUMBER_OF_TRITS_IN_A_BYTE;
                    j-- > 0;)
                {
                    value = value*RADIX + trits[offset + i*NUMBER_OF_TRITS_IN_A_BYTE + j];
                }
                bytes[i] = (byte) value;
            }

            return bytes;
        }

        public static byte[] bytes(int[] trits)
        {
            return bytes(trits, 0, trits.Length);
        }

        public static void getTrits(sbyte[] bytes, int[] trits)
        {
            int offset = 0;
            for (int i = 0; i < bytes.Length && offset < trits.Length; i++)
            {
                // TODO: not sure if correct because bytes are encoded differently in java
                Array.Copy(
                    BYTE_TO_TRITS_MAPPINGS[bytes[i] < 0 ? (bytes[i] + BYTE_TO_TRITS_MAPPINGS.Length) : bytes[i]], 0,
                    trits, offset,
                    trits.Length - offset < NUMBER_OF_TRITS_IN_A_BYTE
                        ? (trits.Length - offset)
                        : NUMBER_OF_TRITS_IN_A_BYTE);

                offset += NUMBER_OF_TRITS_IN_A_BYTE;
            }
            while (offset < trits.Length)
            {
                trits[offset++] = 0;
            }
        }

        public static int[] tritsString(String trytes)
        {
            int[] d = new int[3*trytes.Length];
            for (int i = 0; i < trytes.Length; i++)
            {
                Array.Copy(TRYTE_TO_TRITS_MAPPINGS[Constants.TRYTE_ALPHABET.IndexOf(trytes[i])], 0, d,
                    i*NUMBER_OF_TRITS_IN_A_TRYTE, NUMBER_OF_TRITS_IN_A_TRYTE);
            }
            return d;
        }

        public static int[] trits(string trytes, int length)
        {
            int[] tritss = trits(trytes);

            List<int> tritsList = new List<int>();

            foreach (int i in tritss)
                tritsList.Add(i);

            while (tritsList.Count < length)
                tritsList.Add(0);

            return tritsList.ToArray();
        }

        public static int[] trits(String trytes)
        {
            List<int> trits = new List<int>();
            if (InputValidator.isValue(trytes))
            {
                long value = long.Parse(trytes);

                long absoluteValue = value < 0 ? -value : value;

                int position = 0;

                while (absoluteValue > 0)
                {
                    int remainder = (int) (absoluteValue%RADIX);
                    absoluteValue /= RADIX;

                    if (remainder > MAX_TRIT_VALUE)
                    {
                        remainder = MIN_TRIT_VALUE;
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
                    Array.Copy(TRYTE_TO_TRITS_MAPPINGS[Constants.TRYTE_ALPHABET.IndexOf(trytes[i])], 0, d,
                        i*NUMBER_OF_TRITS_IN_A_TRYTE, NUMBER_OF_TRITS_IN_A_TRYTE);
                }
                return d;
            }
            return trits.ToArray();
        }

        public static int[] copyTrits(string input, int[] destination)
        {
            for (int i = 0; i < input.Length; i++)
            {
                int index = Constants.TRYTE_ALPHABET.IndexOf(input[i]);
                destination[i*3] = TRYTE_TO_TRITS_MAPPINGS[index][0];
                destination[i*3 + 1] = TRYTE_TO_TRITS_MAPPINGS[index][1];
                destination[i*3 + 2] = TRYTE_TO_TRITS_MAPPINGS[index][2];
            }
            return destination;
        }

        public static void copyTrits(long value, int[] destination, int offset, int size)
        {
            long absoluteValue = value < 0 ? -value : value;
            for (int i = 0; i < size; i++)
            {
                int remainder = (int) (absoluteValue%RADIX);
                absoluteValue /= RADIX;
                if (remainder > MAX_TRIT_VALUE)
                {
                    remainder = MIN_TRIT_VALUE;
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

        public static string trytes(int[] trits, int offset, int size)
        {
            StringBuilder trytes = new StringBuilder();
            for (int i = 0; i < (size + NUMBER_OF_TRITS_IN_A_TRYTE - 1)/NUMBER_OF_TRITS_IN_A_TRYTE; i++)
            {
                int j = trits[offset + i*3] + trits[offset + i*3 + 1]*3 + trits[offset + i*3 + 2]*9;
                if (j < 0)
                {
                    j += TRYTE_ALPHABET.Length;
                }
                trytes.Append(TRYTE_ALPHABET[j]);
            }
            return trytes.ToString();
        }

        public static string trytes(int[] trits)
        {
            return trytes(trits, 0, trits.Length);
        }

        public static int tryteValue(int[] trits, int offset)
        {
            return trits[offset] + trits[offset + 1]*3 + trits[offset + 2]*9;
        }

        public static int value(int[] trits)
        {
            int value = 0;

            for (int i = trits.Length; i-- > 0;)
            {
                value = value*3 + trits[i];
            }
            return value;
        }

        public static long longValue(int[] trits)
        {
            long value = 0;

            for (int i = trits.Length; i-- > 0;)
            {
                value = value*3 + trits[i];
            }
            return value;
        }

        public static void increment(int[] trits, int size)
        {
            for (int i = 0; i < size; i++)
            {
                if (++trits[i] > Converter.MAX_TRIT_VALUE)
                {
                    trits[i] = Converter.MIN_TRIT_VALUE;
                }
                else
                {
                    break;
                }
            }
        }

        // TODO: Move to Transaction class?
        public static string transactionTrytes(Transaction trx)
        {
            int[] valueTrits = Converter.trits(trx.Value, 81);
            int[] timestampTrits = Converter.trits(trx.Timestamp, 27);
            int[] currentIndexTrits = Converter.trits(trx.CurrentIndex, 27);
            int[] lastIndexTrits = Converter.trits(trx.LastIndex, 27);

            return trx.SignatureFragment
                   + trx.Address
                   + Converter.trytes(valueTrits)
                   + trx.Tag
                   + Converter.trytes(timestampTrits)
                   + Converter.trytes(currentIndexTrits)
                   + Converter.trytes(lastIndexTrits)
                   + trx.Bundle
                   + trx.TrunkTransaction
                   + trx.BranchTransaction
                   + trx.Nonce;
        }
    }
}