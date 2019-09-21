﻿using System;
using System.Collections.Generic;
using System.Linq;
using IotaSharp.Model;
using IotaSharp.Pow;

namespace IotaSharp.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class Signing
    {
        private readonly ICurl _curl;

        /// <summary>
        /// </summary>
        /// <param name="curl"></param>
        public Signing(ICurl curl)
        {
            _curl = curl ?? new Kerl();
        }

        /// <summary>
        /// </summary>
        public Signing()
        {
            _curl = new Kerl();
        }

        /// <summary>
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="index"></param>
        /// <param name="security"></param>
        /// <returns></returns>
        public sbyte[] Key(sbyte[] seed, int index, int security)
        {
            var subSeed = new sbyte[seed.Length];
            seed.CopyTo(subSeed, 0);

            for (var i = 0; i < index; i++)
            for (var j = 0; j < 243; j++)
                if (++subSeed[j] > 1)
                    subSeed[j] = -1;
                else
                    break;

            _curl.Reset();
            _curl.Absorb(subSeed, 0, subSeed.Length);
            _curl.Squeeze(subSeed, 0, subSeed.Length);
            _curl.Reset();
            _curl.Absorb(subSeed, 0, subSeed.Length);

            var key = new List<sbyte>();
            var buffer = new sbyte[subSeed.Length];
            var offset = 0;

            while (security-- > 0)
                for (var i = 0; i < 27; i++)
                {
                    _curl.Squeeze(buffer, offset, buffer.Length);
                    for (var j = 0; j < 243; j++) key.Add(buffer[j]);
                }

            return key.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public sbyte[] Digests(sbyte[] key)
        {
            var digests = new sbyte[(int) Math.Floor((decimal) key.Length / 6561) * 243];
            var buffer = new sbyte[243];

            for (var i = 0; i < Math.Floor((decimal) key.Length / 6561); i++)
            {
                var keyFragment = new sbyte[6561];
                Array.Copy(key, i * 6561, keyFragment, 0, 6561);

                for (var j = 0; j < 27; j++)
                {
                    Array.Copy(keyFragment, j * 243, buffer, 0, 243);
                    for (var k = 0; k < 26; k++)
                    {
                        _curl.Reset();
                        _curl.Absorb(buffer, 0, buffer.Length);
                        _curl.Squeeze(buffer, 0, buffer.Length);
                    }

                    for (var k = 0; k < 243; k++) keyFragment[j * 243 + k] = buffer[k];
                }

                _curl.Reset();
                _curl.Absorb(keyFragment, 0, keyFragment.Length);
                _curl.Squeeze(buffer, 0, buffer.Length);

                for (var j = 0; j < 243; j++) digests[i * 243 + j] = buffer[j];
            }

            return digests;
        }

        /// <summary>
        /// </summary>
        /// <param name="normalizedBundleFragment"></param>
        /// <param name="signatureFragment"></param>
        /// <returns></returns>
        public sbyte[] Digest(sbyte[] normalizedBundleFragment, sbyte[] signatureFragment)
        {
            _curl.Reset();
            sbyte[] buffer = null;

            for (var i = 0; i < 27; i++)
            {
                buffer = ArrayUtils.SubArray(signatureFragment, i * 243, (i + 1) * 243);

                var jCurl = new Kerl();

                for (var j = normalizedBundleFragment[i] + 13; j-- > 0;)
                {
                    jCurl.Reset();
                    jCurl.Absorb(buffer);
                    jCurl.Squeeze(buffer);
                }

                _curl.Absorb(buffer);
            }

            _curl.Squeeze(buffer);

            return buffer;
        }

        /// <summary>
        /// </summary>
        /// <param name="digests"></param>
        /// <returns></returns>
        public sbyte[] Address(sbyte[] digests)
        {
            var address = new sbyte[243];
            _curl.Reset();
            _curl.Absorb(digests, 0, digests.Length);
            _curl.Squeeze(address, 0, address.Length);
            return address;
        }

        /// <summary>
        /// </summary>
        /// <param name="normalizedBundleFragment"></param>
        /// <param name="keyFragment"></param>
        /// <returns></returns>
        public sbyte[] SignatureFragment(sbyte[] normalizedBundleFragment, sbyte[] keyFragment)
        {
            var hash = new sbyte[243];

            for (var i = 0; i < 27; i++)
            {
                Array.Copy(keyFragment, i * 243, hash, 0, 243);

                for (var j = 0; j < 13 - normalizedBundleFragment[i]; j++)
                {
                    _curl.Reset();
                    _curl.Absorb(hash, 0, hash.Length);
                    _curl.Squeeze(hash, 0, hash.Length);
                }

                //for (var j = 0; j < 243; j++) Array.Copy(hash, j, keyFragment, i * 243 + j, 1);
                Array.Copy(hash, 0, keyFragment, i * 243, 243);
            }

            return keyFragment;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="signedBundle"></param>
        /// <param name="inputAddress"></param>
        /// <returns></returns>
        public bool ValidateSignatures(Bundle signedBundle, string inputAddress)
        {
            var bundleHash = "";

            var signatureFragments = new List<string>();

            foreach (var trx in signedBundle.Transactions)
            {
                if (trx.Address.Equals(inputAddress, StringComparison.Ordinal))
                {
                    bundleHash = trx.Bundle;

                    // if we reached remainder bundle
                    var signatureFragment = trx.SignatureMessageFragment;
                    if (InputValidator.IsNinesTrytes(signatureFragment, signatureFragment.Length))
                    {
                        break;
                    }

                    signatureFragments.Add(signatureFragment);
                }
            }

            return ValidateSignatures(inputAddress, signatureFragments.ToArray(), bundleHash);
        }

        /// <summary>
        /// </summary>
        /// <param name="expectedAddress"></param>
        /// <param name="signatureFragments"></param>
        /// <param name="bundleHash"></param>
        /// <returns></returns>
        public bool ValidateSignatures(string expectedAddress, string[] signatureFragments, string bundleHash)
        {
            var bundle = new Bundle();

            var normalizedBundleFragments = new sbyte[3, 27];
            var normalizedBundleHash = bundle.NormalizedBundle(bundleHash);

            // Split hash into 3 fragments
            for (var i = 0; i < 3; i++)
            {
                //normalizedBundleFragments[i] = Arrays.copyOfRange(normalizedBundleHash, i*27, (i + 1)*27);
                //Array.Copy(normalizedBundleHash, i * 27, normalizedBundleFragments, 0, 27);
                for (var j = 0; j < 27; j++)
                {
                    normalizedBundleFragments[i, j] = normalizedBundleHash[i * 27 + j];
                }
            }


            // Get digests
            var digests = new sbyte[signatureFragments.Length * 243];

            for (var i = 0; i < signatureFragments.Length; i++)
            {
                var digestBuffer = Digest(ArrayUtils.SliceRow(normalizedBundleFragments, i % 3).ToArray(),
                    Converter.ToTrits(signatureFragments[i]));

                Array.Copy(digestBuffer, 0, digests, i * 243, 243);
            }

            var address = Converter.ToTrytes(Address(digests));

            return expectedAddress.Equals(address, StringComparison.Ordinal);
        }
    }
}
