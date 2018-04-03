using System;
using Iesi.Collections.Generic;
using Iota.Api.Exception;

namespace Iota.Api.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class FixedBigIntConverter
    {
        //243:3 384:2
        private const int TritsLength = 243;

        private const int BitLength = 384;
        private const int ByteLength = BitLength / 8;
        private const int IntLength = ByteLength / 4;

        /// hex representation of (3^242-1)/2
        private static readonly int[] Half3 =
        {
            unchecked((int) 0xa5ce8964), unchecked ((int) 0x9f007669),
            0x1484504f, 0x3ade00d9,
            0x0c24486e, 0x50979d57,
            0x79a4c702, 0x48bbae36,
            unchecked ((int) 0xa9f6808b), unchecked ((int) 0xaa06a805),
            unchecked ((int) 0xa87fabdf), 0x5e69ebef
        };

        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="bytes"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void FromTritsToBytes(int[] trits, byte[] bytes)
        {
            if (trits.Length != TritsLength)
                throw new ArgumentException("trits array has invalid size");

            if (bytes.Length != ByteLength)
                throw new ArgumentException("bytes array has invalid size");

            var baseHalf3 = new int[IntLength];

            var setUniqueNumbers = new LinkedHashSet<int>();
            foreach (var x in trits) setUniqueNumbers.Add(x);

            if (setUniqueNumbers.Count == 1 && setUniqueNumbers.Contains(-1))
            {
                baseHalf3 = (int[]) Half3.Clone();
                BigIntNot(baseHalf3);
                BigIntAdd(baseHalf3, 1);
            }
            else
            {
                var size = IntLength;
                for (var i = TritsLength - 1; i-- > 0;)
                {
                    {
                        // Multiply by radix
                        var sz = size;
                        var carry = 0;

                        for (var j = 0; j < sz; j++)
                        {
                            // full_mul
                            var v = ToUnsignedLong(baseHalf3[j]) * 3 +
                                    ToUnsignedLong(carry);
                            carry = (int) ((v >> (sizeof(int) * 8)) & 0xFFFFFFFF);
                            baseHalf3[j] = (int) (v & 0xFFFFFFFF);
                        }

                        if (carry > 0)
                        {
                            baseHalf3[sz] = carry;
                            size += 1;
                        }
                    }

                    var inValue = trits[i] + 1;
                    {
                        // Add
                        var sz = BigIntAdd(baseHalf3, inValue);
                        if (sz > size) size = sz;
                    }
                }

                if (Sum(baseHalf3) != 0)
                    if (BigIntCmp(Half3, baseHalf3) <= 0)
                    {
                        // base is >= HALF_3.
                        // just do base - HALF_3
                        baseHalf3 = BigIntSub(baseHalf3, Half3);
                    }
                    else
                    {
                        // we don't have a wrapping sub.
                        // so we need to be clever.
                        baseHalf3 = BigIntSub(Half3, baseHalf3);
                        BigIntNot(baseHalf3);
                        BigIntAdd(baseHalf3, 1);
                    }
            }


            // output
            for (var i = 0; i < IntLength; i++)
            {
                bytes[i * 4 + 0] = (byte) ((baseHalf3[IntLength - 1 - i] & 0xFF000000) >> 24);
                bytes[i * 4 + 1] = (byte) ((baseHalf3[IntLength - 1 - i] & 0x00FF0000) >> 16);
                bytes[i * 4 + 2] = (byte) ((baseHalf3[IntLength - 1 - i] & 0x0000FF00) >> 8);
                bytes[i * 4 + 3] = (byte) ((baseHalf3[IntLength - 1 - i] & 0x000000FF) >> 0);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="trits"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void FromBytesToTrits(byte[] bytes, int[] trits)
        {
            if (bytes.Length != ByteLength)
                throw new ArgumentException("bytes array has invalid size");

            if (trits.Length != TritsLength)
                throw new ArgumentException("trits array has invalid size");

            var bigIntValue = new int[IntLength];

            for (var i = 0; i < IntLength; i++)
            {
                bigIntValue[IntLength - 1 - i] = ToUnsignedInt(bytes[i * 4]) << 24;
                bigIntValue[IntLength - 1 - i] |= ToUnsignedInt(bytes[i * 4 + 1]) << 16;
                bigIntValue[IntLength - 1 - i] |= ToUnsignedInt(bytes[i * 4 + 2]) << 8;
                bigIntValue[IntLength - 1 - i] |= ToUnsignedInt(bytes[i * 4 + 3]);
            }

            if (BigIntCmp(bigIntValue, Half3) == 0)
            {
                var val = 0;
                if (bigIntValue[0] > 0)
                    val = -1;
                else if (bigIntValue[0] < 0) val = 1;

                for (var i = 0; i < TritsLength - 1; i++) trits[i] = val;
            }
            else
            {
                var flipTrits = false;
                // See if we have a positive or negative two's complement number.
                if (ToUnsignedLong(bigIntValue[IntLength - 1]) >> 31 != 0)
                {
                    // negative value.
                    BigIntNot(bigIntValue);
                    if (BigIntCmp(bigIntValue, Half3) > 0)
                    {
                        bigIntValue = BigIntSub(bigIntValue, Half3);
                        flipTrits = true;
                    }
                    else
                    {
                        BigIntAdd(bigIntValue, 1);
                        bigIntValue = BigIntSub(Half3, bigIntValue);
                    }
                }
                else
                {
                    // positive. we need to shift right by HALF_3
                    bigIntValue = BigIntAdd(Half3, bigIntValue);
                }

                var size = IntLength;

                for (var i = 0; i < TritsLength - 1; i++)
                {
                    int remainder;
                    {
                        //div_rem
                        remainder = 0;

                        for (var j = size - 1; j >= 0; j--)
                        {
                            var lhs = (ToUnsignedLong(remainder) << 32) | ToUnsignedLong(bigIntValue[j]);
                            var rhs = 3;

                            var q = (int) (lhs / rhs);
                            var r = (int) (lhs % rhs);
                            bigIntValue[j] = q;
                            remainder = r;
                        }
                    }
                    trits[i] = remainder - 1;
                }

                if (flipTrits)
                    for (var i = 0; i < TritsLength; i++)
                        trits[i] = -trits[i];
            }
        }


        #region Private Method

        private static int ToUnsignedInt(byte x)
        {
            return x & 0xff;
        }

        private static long ToUnsignedLong(int i)
        {
            return i & 0xFFFFFFFFL;
        }

        private static int Sum(int[] toSum)
        {
            var sum = 0;
            foreach (var t in toSum) sum += t;

            return sum;
        }

        private static void BigIntNot(int[] baseValue)
        {
            for (var i = 0; i < baseValue.Length; i++) baseValue[i] = ~baseValue[i];
        }

        private static int BigIntAdd(int[] baseValue, int rh)
        {
            var res = FullAdd(baseValue[0], rh, false);
            baseValue[0] = res.Item1;

            var j = 1;
            while (res.Item2)
            {
                res = FullAdd(baseValue[j], 0, true);
                baseValue[j] = res.Item1;
                j += 1;
            }

            return j;
        }

        private static int[] BigIntAdd(int[] lh, int[] rh)
        {
            var outValue = new int[IntLength];
            var carry = false;
            for (var i = 0; i < IntLength; i++)
            {
                var ret = FullAdd(lh[i], rh[i], carry);
                outValue[i] = ret.Item1;
                carry = ret.Item2;
            }

            if (carry) throw new IllegalStateException("Exceeded max value.");

            return outValue;
        }

        private static int BigIntCmp(int[] lh, int[] rh)
        {
            for (var i = IntLength - 1; i >= 0; i--)
            {
                var a = ToUnsignedLong(lh[i]);
                var b = ToUnsignedLong(rh[i]);

                var ret = a.CompareTo(b);
                if (ret != 0) return ret;
            }

            return 0;
        }

        private static int[] BigIntSub(int[] lh, int[] rh)
        {
            var outValue = new int[IntLength];
            var noborrow = true;

            for (var i = 0; i < IntLength; i++)
            {
                var ret = FullAdd(lh[i], ~rh[i], noborrow);
                outValue[i] = ret.Item1;
                noborrow = ret.Item2;
            }

            if (!noborrow) throw new IllegalStateException("noborrow");

            return outValue;
        }

        private static Tuple<int, bool> FullAdd(int ia, int ib, bool carry)
        {
            var a = ToUnsignedLong(ia);
            var b = ToUnsignedLong(ib);

            var v = a + b;
            var l = v >> 32;
            var r = v & 0xFFFFFFFF;
            var carry1 = l != 0;

            if (carry) v = r + 1;

            l = (v >> 32) & 0xFFFFFFFF;
            r = v & 0xFFFFFFFF;
            var carry2 = l != 0;

            return new Tuple<int, bool>((int) r, carry1 || carry2);
        }

        #endregion
    }
}