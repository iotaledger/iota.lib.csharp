using System;
using Iota.Api.Pow;
using Iota.MAM.Utils;

namespace Iota.MAM.Core
{
    public class MaskHelper
    {
        public static void Mask(int[] payload, int[] key, ICurl curl)
        {
            Mask(payload, 0, payload.Length, key, 0, key.Length, curl);
        }

        public static void Mask(
            int[] payload, int payloadIndex, int payloadLength,
            int[] key, int keyIndex, int keyLength,
            ICurl curl)
        {
            curl.Absorb(key, keyIndex, keyLength);
            var keyChunk = new int[Constants.HashLength];
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

        public static void UnMask(int[] payload, int[] key, ICurl curl)
        {
            UnMask(payload, 0, payload.Length, key, 0, key.Length, curl);
        }

        public static void UnMask(
            int[] payload, int payloadIndex, int payloadLength,
            int[] key, int keyIndex, int keyLength,
            ICurl curl)
        {
            curl.Absorb(key, keyIndex, keyLength);
            var keyChunk = new int[Constants.HashLength];
            curl.Squeeze(keyChunk);

            for (var i = 0; i < payloadLength; i += Constants.HashLength)
            {
                var left = payloadLength - i;
                var length = left > Constants.HashLength ? Constants.HashLength : left;

                for (var n = 0; n < length; n++)
                    keyChunk[n] = TritsHelper.Sum(payload[payloadIndex + i + n], -keyChunk[n]);

                Array.Copy(keyChunk, 0, payload, payloadIndex + i, length);
                curl.Absorb(keyChunk, 0, length);

                Array.Copy(curl.Rate, keyChunk, length);
            }
        }

        public static void MaskSlice(int[] payload, ICurl curl)
        {
            MaskSlice(payload, 0, payload.Length, curl);
        }

        public static void MaskSlice(int[] payload, int payloadIndex, int payloadLength, ICurl curl)
        {
            var keyChunk = new int[Constants.HashLength];
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

        public static void UnMaskSlice(int[] payload, ICurl curl)
        {
            UnMaskSlice(payload, 0, payload.Length, curl);
        }

        public static void UnMaskSlice(int[] payload, int payloadIndex, int payloadLength, ICurl curl)
        {
            var keyChunk = new int[Constants.HashLength];

            for (var i = 0; i < payloadLength; i += Constants.HashLength)
            {
                var left = payloadLength - i;
                var length = left > Constants.HashLength ? Constants.HashLength : left;

                Array.Copy(curl.Rate, keyChunk, length);

                for (var n = 0; n < length; n++)
                    payload[payloadIndex + i + n] = TritsHelper.Sum(payload[payloadIndex + i + n], -keyChunk[n]);

                curl.Absorb(payload, payloadIndex + i, length);
            }
        }
    }
}