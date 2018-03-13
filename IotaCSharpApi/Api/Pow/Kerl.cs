﻿using System;
using System.Linq;
using Iesi.Collections.Generic;
using Iota.Lib.CSharp.Api.Exception;
using Org.BouncyCastle.Crypto.Digests;

namespace Iota.Lib.CSharp.Api.Pow
{
    /// <summary>
    /// 
    /// </summary>
    public class Kerl : Curl
    {
        private new const int HashLength = 243;
        private const int BitHashLength = 384;
        private const int ByteHashLength = BitHashLength / 8;

        private const int Radix = 3;
        private const int MaxTritValue = (Radix - 1) / 2;
        // ReSharper disable once UnusedMember.Local
        private const int MinTritValue = -MaxTritValue;

        private readonly KeccakDigest _keccak;
        private readonly byte[] _byteState;
        private int[] _tritState;

        private static readonly int[] Half3 = {
            unchecked((int) 0xa5ce8964), unchecked ((int) 0x9f007669),
            0x1484504f, 0x3ade00d9,
            0x0c24486e, 0x50979d57,
            0x79a4c702, 0x48bbae36,
            unchecked ((int) 0xa9f6808b), unchecked ((int) 0xaa06a805),
            unchecked ((int) 0xa87fabdf), 0x5e69ebef
        };


        private const int ByteLength = 48;
        private const int IntLength = ByteLength / 4;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public Kerl() : base(CurlMode.CurlP81)
        {
            _keccak = new KeccakDigest(BitHashLength);
            _byteState = new byte[ByteHashLength];
            _tritState = new int[HashLength];
        }

        private static long ToUnsignedLong(int i)
        {
            return i & 0xFFFFFFFFL;
        }

        // ReSharper disable once UnusedMember.Local
        private static int ToUnsignedInt(byte x)
        {
            return x & 0xff;
        }

        private static int Sum(int[] toSum)
        {
            int sum = 0;
            foreach (var t in toSum)
            {
                sum += t;
            }

            return sum;
        }

        private static void BigintNot(int[] baseValue)
        {
            for (int i = 0; i < baseValue.Length; i++)
            {
                baseValue[i] = ~baseValue[i];
            }
        }

        private static int BigintAdd(int[] baseValue, int rh)
        {
            Tuple<int, bool> res = FullAdd(baseValue[0], rh, false);
            baseValue[0] = res.Item1;

            int j = 1;
            while (res.Item2)
            {
                res = FullAdd(baseValue[j], 0, true);
                baseValue[j] = res.Item1;
                j += 1;
            }

            return j;
        }

        // ReSharper disable once UnusedMember.Local
        private static int[] BigintAdd(int[] lh, int[] rh)
        {
            int[] outValue = new int[IntLength];
            bool carry = false;
            for (int i = 0; i < IntLength; i++)
            {
                var ret = FullAdd(lh[i], rh[i], carry);
                outValue[i] = ret.Item1;
                carry = ret.Item2;
            }

            if (carry)
            {
                throw new IllegalStateException("Exceeded max value.");
            }

            return outValue;
        }

        private static int BigintCmp(int[] lh, int[] rh)
        {
            for (int i = IntLength - 1; i >= 0; i--)
            {
                var a = ToUnsignedLong(lh[i]);
                var b = ToUnsignedLong(rh[i]);

                int ret = a.CompareTo(b);
                if (ret != 0)
                {
                    return ret;
                }
            }

            return 0;
        }

        private static int[] BigintSub(int[] lh, int[] rh)
        {
            int[] outValue = new int[IntLength];
            bool noborrow = true;

            for (int i = 0; i < IntLength; i++)
            {
                var ret = FullAdd(lh[i], ~rh[i], noborrow);
                outValue[i] = ret.Item1;
                noborrow = ret.Item2;
            }

            if (!noborrow)
            {
                throw new IllegalStateException("noborrow");
            }

            return outValue;
        }

        private static Tuple<int, bool> FullAdd(int ia, int ib, bool carry)
        {
            long a = ToUnsignedLong(ia);
            long b = ToUnsignedLong(ib);

            long v = a + b;
            long l = v >> 32;
            long r = v & 0xFFFFFFFF;
            bool carry1 = l != 0;

            if (carry)
            {
                v = r + 1;
            }

            l = (v >> 32) & 0xFFFFFFFF;
            r = v & 0xFFFFFFFF;
            bool carry2 = l != 0;

            return new Tuple<int, bool>((int) r, carry1 || carry2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] ConvertTritsToBytes(int[] trits)
        {
            if (trits.Length != HashLength)
            {
                throw new ArgumentException("Input trits length must be " + HashLength + "in length");
            }

            int[] baseHalf3 = new int[IntLength];

            var setUniqueNumbers = new LinkedHashSet<int>();
            foreach (var x in trits)
            {
                setUniqueNumbers.Add(x);
            }

            if (setUniqueNumbers.Count() == 1 && setUniqueNumbers.Contains(-1))
            {
                baseHalf3 = (int[]) Half3.Clone();
                BigintNot(baseHalf3);
                BigintAdd(baseHalf3, 1);
            }
            else
            {
                int size = IntLength;
                for (int i = HashLength - 1; i-- > 0;)
                {
                    {
                        // Multiply by radix
                        int sz = size;
                        int carry = 0;

                        for (int j = 0; j < sz; j++)
                        {
                            // full_mul
                            long v = ToUnsignedLong(baseHalf3[j]) * (ToUnsignedLong(Radix)) +
                                     ToUnsignedLong(carry);
                            carry = (int) ((v >> (sizeof(int)*8)) & 0xFFFFFFFF);
                            baseHalf3[j] = (int) (v & 0xFFFFFFFF);
                        }

                        if (carry > 0)
                        {
                            baseHalf3[sz] = carry;
                            size += 1;
                        }
                    }

                    int inValue = trits[i] + 1;
                    {
                        // Add
                        int sz = BigintAdd(baseHalf3, inValue);
                        if (sz > size)
                        {
                            size = sz;
                        }
                    }
                }

                if (Sum(baseHalf3) != 0)
                {
                    if (BigintCmp(Half3, baseHalf3) <= 0)
                    {
                        // base is >= HALF_3.
                        // just do base - HALF_3
                        baseHalf3 = BigintSub(baseHalf3, Half3);
                    }
                    else
                    {
                        // we don't have a wrapping sub.
                        // so we need to be clever.
                        baseHalf3 = BigintSub(Half3, baseHalf3);
                        BigintNot(baseHalf3);
                        BigintAdd(baseHalf3, 1);
                    }
                }

            }

            byte[] outValue = new byte[ByteLength];

            for (int i = 0; i < IntLength; i++)
            {
                outValue[i * 4 + 0] = (byte) ((baseHalf3[IntLength - 1 - i] & 0xFF000000) >> 24);
                outValue[i * 4 + 1] = (byte) ((baseHalf3[IntLength - 1 - i] & 0x00FF0000) >> 16);
                outValue[i * 4 + 2] = (byte) ((baseHalf3[IntLength - 1 - i] & 0x0000FF00) >> 8);
                outValue[i * 4 + 3] = (byte) ((baseHalf3[IntLength - 1 - i] & 0x000000FF) >> 0);
            }

            return outValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// <exception cref="IllegalStateException"></exception>
        public static int[] ConvertBytesToTrits(byte[] bytes)
        {
            int[] baseValue = new int[IntLength];
            int[] outValue = new int[243];
            outValue[HashLength - 1] = 0;

            if (bytes.Length != ByteLength)
            {
                throw new IllegalStateException("Input base must be " + ByteLength + " in length");
            }

            for (int i = 0; i < IntLength; i++)
            {
                baseValue[IntLength - 1 - i] = ToUnsignedInt(bytes[i * 4]) << 24;
                baseValue[IntLength - 1 - i] |= ToUnsignedInt(bytes[i * 4 + 1]) << 16;
                baseValue[IntLength - 1 - i] |= ToUnsignedInt(bytes[i * 4 + 2]) << 8;
                baseValue[IntLength - 1 - i] |= ToUnsignedInt(bytes[i * 4 + 3]);
            }

            if (BigintCmp(baseValue, Half3) == 0)
            {
                int val = 0;
                if (baseValue[0] > 0)
                {
                    val = -1;
                }
                else if (baseValue[0] < 0)
                {
                    val = 1;
                }

                for (int i = 0; i < HashLength - 1; i++)
                {
                    outValue[i] = val;
                }

            }
            else
            {
                bool flipTrits = false;
                // See if we have a positive or negative two's complement number.
                if (ToUnsignedLong(baseValue[IntLength - 1]) >> 31 != 0)
                {
                    // negative value.
                    BigintNot(baseValue);
                    if (BigintCmp(baseValue, Half3) > 0)
                    {
                        baseValue = BigintSub(baseValue, Half3);
                        flipTrits = true;
                    }
                    else
                    {
                        BigintAdd(baseValue, 1);
                        baseValue = BigintSub(Half3, baseValue);
                    }
                }
                else
                {
                    // positive. we need to shift right by HALF_3
                    baseValue = BigintAdd(Half3, baseValue);
                }

                int size = IntLength;

                for (int i = 0; i < HashLength - 1; i++)
                {
                    int remainder;
                    {
                        //div_rem
                        remainder = 0;

                        for (int j = size - 1; j >= 0; j--)
                        {
                            long lhs = (ToUnsignedLong(remainder) << 32) | ToUnsignedLong(baseValue[j]);
                            long rhs = ToUnsignedLong(Radix);

                            int q = (int) (lhs / rhs);
                            int r = (int) (lhs % rhs);
                            baseValue[j] = q;
                            remainder = r;
                        }
                    }
                    outValue[i] = remainder - 1;
                }

                if (flipTrits)
                {
                    for (int i = 0; i < outValue.Length; i++)
                    {
                        outValue[i] = -outValue[i];
                    }
                }
            }

            return outValue;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override ICurl Reset()
        {
            _keccak.Reset();

            return this;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override ICurl Absorb(int[] trits, int offset, int length)
        {

            if (length % 243 != 0)
                throw new IllegalStateException("Illegal length: " + length);

            do
            {

                //copy trits[offset:offset+length]
                Array.Copy(trits, offset, _tritState, 0, HashLength);

                //convert to bits
                _tritState[HashLength - 1] = 0;
                byte[] bytes = ConvertTritsToBytes(_tritState);

                //run keccak
                _keccak.BlockUpdate(bytes, 0, bytes.Length);
                offset += HashLength;

            } while ((length -= HashLength) > 0);

            return this;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="T:Iota.Lib.CSharp.Api.Exception.IllegalStateException"></exception>
        public override int[] Squeeze(int[] trits, int offset, int length)
        {

            if (length % 243 != 0) throw new IllegalStateException("Illegal length: " + length);

            do
            {
                _keccak.DoFinal(_byteState, 0);
                
                //convert to trits
                _tritState = ConvertBytesToTrits(_byteState);

                //copy with offset
                _tritState[HashLength - 1] = 0;
                Array.Copy(_tritState, 0, trits, offset, HashLength);

                //calculate hash again
                for (int i = _byteState.Length; i-- > 0;)
                {
                    _byteState[i] = (byte) (_byteState[i] ^ 0xFF);
                }

                _keccak.BlockUpdate(_byteState, 0, _byteState.Length);
                offset += HashLength;

            } while ((length -= HashLength) > 0);

            return trits;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        public override int[] Squeeze(int[] trits) => Squeeze(trits, 0, trits.Length);

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override ICurl Clone() => new Kerl();
    }
}
