using System;
using Org.BouncyCastle.Math;

namespace Iota.Api.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class BigIntConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static BigInteger BigIntFromTrits(int[] trits, int offset, int size)
        {
            var value = BigInteger.Zero;

            for (var i = size; i-- > 0;)
                value = value.Multiply(BigInteger.ValueOf(Converter.Radix)).Add(BigInteger.ValueOf(trits[offset + i]));

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static BigInteger BigIntFromBytes(byte[] bytes, int offset, int size)
        {
            return new BigInteger(bytes, offset, size);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destination"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void TritsFromBigInt(BigInteger value, int[] destination, int offset, int size)
        {
            if (destination.Length - offset < size) throw new ArgumentException("Destination array has invalid size");

            var absoluteValue = value.CompareTo(BigInteger.Zero) < 0 ? value.Negate() : value;
            for (var i = 0; i < size; i++)
            {
                var divRemainder = absoluteValue.DivideAndRemainder(BigInteger.ValueOf(Converter.Radix));
                var remainder = divRemainder[1].IntValue;
                absoluteValue = divRemainder[0];

                if (remainder > Converter.MaxTritValue)
                {
                    remainder = Converter.MinTritValue;
                    absoluteValue = absoluteValue.Add(BigInteger.One);
                }

                destination[offset + i] = remainder;
            }

            if (value.CompareTo(BigInteger.Zero) < 0)
                for (var i = 0; i < size; i++)
                    destination[offset + i] = -destination[offset + i];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destination"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void BytesFromBigInt(BigInteger value, byte[] destination, int offset,int size)
        {
            if (destination.Length - offset < size)
                throw new ArgumentException("Destination array has invalid size.");

            var bytes = value.ToByteArray();
            var i = 0;
            while (i + bytes.Length < size) destination[i++] = (byte) ((sbyte) bytes[0] < 0 ? -1 : 0);

            for (var j = bytes.Length; j-- > 0;) destination[i++] = bytes[bytes.Length - 1 - j];
        }
    }
}
