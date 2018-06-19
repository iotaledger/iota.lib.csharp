using System;
using Iota.Api.Pow;
using Iota.Api.Utils;

namespace Iota.MAM.Utils
{
    public class HashHelper
    {
        public static string Hash(string trytes)
        {
            Sponge curl = new Curl(CurlMode.CurlP81);
            var trits = Converter.ToTrits(trytes);
            var hashTrits = new int[Sponge.HashLength];

            curl.Absorb(trits, 0, trits.Length);
            curl.Squeeze(hashTrits, 0, Sponge.HashLength);
            curl.Reset();

            return Converter.ToTrytes(hashTrits);
        }

        public static int[] Subseed(int[] seed, int index, ICurl curl)
        {
            int[] subseed = new int[Constants.HashLength];
            int copyLength = Math.Min(seed.Length, subseed.Length);
            Array.Copy(seed, subseed, copyLength);

            for (var i = 0; i < index; i++)
            for (var j = 0; j < Constants.HashLength; j++)
                if (++subseed[j] > 1)
                    subseed[j] = -1;
                else
                    break;

            curl.Absorb(subseed); // curl.absorb(&out[0..seed.len()]);
            curl.Squeeze(subseed); // curl.squeeze(&mut out[0..HASH_LENGTH]);
            curl.Reset();

            return subseed;
        }

        public static int[] SubseedToDigest(
            int[] subseed, int security,
            ICurl c1, ICurl c2, ICurl c3)
        {
            int[] digest = new int[Constants.HashLength];

            int length = security * Constants.KeyLength / Constants.HashLength;

            c1.Absorb(subseed);

            for (int i = 0; i < length; i++)
            {
                c1.Squeeze(digest);

                for (int n = 0; n < 27; n++)
                {
                    c2.Reset();
                    c2.Absorb(digest);
                    Array.Copy(c2.State, digest, Constants.HashLength);
                }

                c3.Absorb(digest);
            }

            c3.Squeeze(digest);

            return digest;
        }

        public static int[] Address(int[] digest, ICurl curl)
        {
            int[] address = new int[Constants.AddressLength];

            curl.Absorb(digest);
            curl.Squeeze(address);
            curl.Reset();

            return address;
        }

        // Take first 243 trits of key_space as subseed, and write key out to key_space
        public static void Key(int[] keySpace, int index, int length, int security, ICurl curl)
        {
            int totalLength = security * Constants.KeyLength;
            if (totalLength != length)
                throw new ArgumentException("Key space size must be equal to security space size");

            curl.Absorb(keySpace, index, Constants.HashLength);
            curl.Squeeze(keySpace, index, length);

            for (int divOffset = 0; divOffset < length / Constants.HashLength; divOffset++)
            {
                int offset = divOffset * Constants.HashLength;
                curl.Reset();
                curl.Absorb(keySpace, index + offset, Constants.HashLength);

                Array.Copy(curl.State, 0, keySpace, index + offset, Constants.HashLength);
            }

            curl.Reset();

        }

        //Takes a bundle and input key key_signature, and writes the signature out to key_signature
        public static void Signature(int[] bundle, int[] keySignature, int index, int length, ICurl curl)
        {
            if (bundle.Length != Constants.HashLength)
                throw new ArgumentException("bundle are not equal HashLength!");

            int totalLength = Constants.KeyLength * CheckSumSecurity(bundle);
            if (totalLength != length)
                throw new ArgumentException("Key space size must be equal to security space size");

            for (int i = 0; i < length / Constants.HashLength; i++)
            {
                int maxCount = Constants.MaxTryteValue -
                               (bundle[i * Constants.TryteWidth] +
                                bundle[i * Constants.TryteWidth + 1] * 3 +
                                bundle[i * Constants.TryteWidth + 2] * 9);

                for (int n = 0; n < maxCount; n++)
                {
                    curl.Reset();
                    curl.Absorb(keySignature, index + i * Constants.HashLength, Constants.HashLength);
                    Array.Copy(curl.State, 0, keySignature, index + i * Constants.HashLength, Constants.HashLength);
                }
            }

            curl.Reset();

        }

        public static int CheckSumSecurity(int[] hash)
        {
            if (hash.Length != Constants.HashLength)
                return 0;

            int sum = 0;
            int end0 = Constants.HashLength / 3;
            int end1 = end0 * 2;

            for (int i = 0; i < end0; i++)
                sum += hash[i];
            if (sum == 0)
                return 1;

            for (int i = end0; i < end1; i++)
                sum += hash[i];
            if (sum == 0)
                return 2;

            for (int i = end1; i < Constants.HashLength; i++)
                sum += hash[i];
            if (sum == 0)
                return 3;

            return 0;

        }

        // Takes an input `signature`, and writes its digest out to the first 243 trits
        public static void DigestBundleSignature(int[] bundle, int[] keySignature, int index, int length, ICurl curl)
        {
            if (bundle.Length != Constants.HashLength)
                throw new ArgumentException("bundle are not equal HashLength!");

            int totalLength = Constants.KeyLength * CheckSumSecurity(bundle);
            if (totalLength != length)
                throw new ArgumentException("Key space size must be equal to security space size");

            for (int i = 0; i < length / Constants.HashLength; i++)
            {
                int maxCount =
                    (bundle[i * Constants.TryteWidth] +
                     bundle[i * Constants.TryteWidth + 1] * 3 +
                     bundle[i * Constants.TryteWidth + 2] * 9) - Constants.MinTryteValue;

                for (int n = 0; n < maxCount; n++)
                {
                    curl.Reset();
                    curl.Absorb(keySignature, index + i * Constants.HashLength, Constants.HashLength);
                    Array.Copy(curl.State, 0, keySignature, index + i * Constants.HashLength, Constants.HashLength);
                }
            }

            curl.Reset();

            curl.Absorb(keySignature, index, length);
        }
    }
}