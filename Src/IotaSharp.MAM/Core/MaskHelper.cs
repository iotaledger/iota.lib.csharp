using System;
using IotaSharp.MAM.Utils;
using IotaSharp.Pow;

namespace IotaSharp.MAM.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class MaskHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="key"></param>
        /// <param name="curl"></param>
        public static void Mask(sbyte[] payload, sbyte[] key, ICurl curl)
        {
            Mask(payload, 0, payload.Length, key, 0, key.Length, curl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="payloadIndex"></param>
        /// <param name="payloadLength"></param>
        /// <param name="key"></param>
        /// <param name="keyIndex"></param>
        /// <param name="keyLength"></param>
        /// <param name="curl"></param>
        public static void Mask(
            sbyte[] payload, int payloadIndex, int payloadLength,
            sbyte[] key, int keyIndex, int keyLength,
            ICurl curl)
        {
            curl.Absorb(key, keyIndex, keyLength);
            var keyChunk = new sbyte[Constants.HashLength];
            curl.Squeeze(keyChunk);

            for (var i = 0; i < payloadLength; i += Constants.HashLength)
            {
                var left = payloadLength - i;
                var length = left > Constants.HashLength ? Constants.HashLength : left;

                for (var n = 0; n < length; n++)
                    keyChunk[n] = TritsHelper.Sum(payload[payloadIndex + i + n], keyChunk[n]);

                curl.Absorb(payload, payloadIndex + i, length);
                Array.Copy(keyChunk, 0, payload, payloadIndex + i, length);
                Array.Copy(curl.Rate, keyChunk, length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="key"></param>
        /// <param name="curl"></param>
        public static void UnMask(sbyte[] payload, sbyte[] key, ICurl curl)
        {
            UnMask(payload, 0, payload.Length, key, 0, key.Length, curl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="payloadIndex"></param>
        /// <param name="payloadLength"></param>
        /// <param name="key"></param>
        /// <param name="keyIndex"></param>
        /// <param name="keyLength"></param>
        /// <param name="curl"></param>
        public static void UnMask(
            sbyte[] payload, int payloadIndex, int payloadLength,
            sbyte[] key, int keyIndex, int keyLength,
            ICurl curl)
        {
            curl.Absorb(key, keyIndex, keyLength);
            var keyChunk = new sbyte[Constants.HashLength];
            curl.Squeeze(keyChunk);

            for (var i = 0; i < payloadLength; i += Constants.HashLength)
            {
                var left = payloadLength - i;
                var length = left > Constants.HashLength ? Constants.HashLength : left;

                for (var n = 0; n < length; n++)
                    keyChunk[n] = TritsHelper.Sum(payload[payloadIndex + i + n], (sbyte) -keyChunk[n]);

                Array.Copy(keyChunk, 0, payload, payloadIndex + i, length);
                curl.Absorb(keyChunk, 0, length);

                Array.Copy(curl.Rate, keyChunk, length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="curl"></param>
        public static void MaskSlice(sbyte[] payload, ICurl curl)
        {
            MaskSlice(payload, 0, payload.Length, curl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="payloadIndex"></param>
        /// <param name="payloadLength"></param>
        /// <param name="curl"></param>
        public static void MaskSlice(sbyte[] payload, int payloadIndex, int payloadLength, ICurl curl)
        {
            var keyChunk = new sbyte[Constants.HashLength];
            Array.Copy(curl.Rate, keyChunk, Constants.HashLength);

            for (var i = 0; i < payloadLength; i += Constants.HashLength)
            {
                var left = payloadLength - i;
                var length = left > Constants.HashLength ? Constants.HashLength : left;

                curl.Absorb(payload, payloadIndex + i, length);

                for (var n = 0; n < length; n++)
                    payload[payloadIndex + i + n] = TritsHelper.Sum(payload[payloadIndex + i + n], keyChunk[n]);

                Array.Copy(curl.Rate, keyChunk, length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="curl"></param>
        public static void UnMaskSlice(sbyte[] payload, ICurl curl)
        {
            UnMaskSlice(payload, 0, payload.Length, curl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="payloadIndex"></param>
        /// <param name="payloadLength"></param>
        /// <param name="curl"></param>
        public static void UnMaskSlice(sbyte[] payload, int payloadIndex, int payloadLength, ICurl curl)
        {
            var keyChunk = new int[Constants.HashLength];

            for (var i = 0; i < payloadLength; i += Constants.HashLength)
            {
                var left = payloadLength - i;
                var length = left > Constants.HashLength ? Constants.HashLength : left;

                Array.Copy(curl.Rate, keyChunk, length);

                for (var n = 0; n < length; n++)
                    payload[payloadIndex + i + n] =
                        TritsHelper.Sum(payload[payloadIndex + i + n], (sbyte) -keyChunk[n]);

                curl.Absorb(payload, payloadIndex + i, length);
            }
        }
    }
}