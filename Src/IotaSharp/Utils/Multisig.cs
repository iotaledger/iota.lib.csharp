using System;
using IotaSharp.Model;
using IotaSharp.Pow;

namespace IotaSharp.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class Multisig
    {
        private readonly ICurl _curl;
        private readonly Signing _signing;

        /// <summary>
        /// </summary>
        /// <param name="curl"></param>
        public Multisig(ICurl curl)
        {
            _curl = curl;
            _curl.Reset();
            _signing = new Signing();
        }

        /// <summary>
        /// </summary>
        public Multisig() : this(new Kerl())
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="seed">Tryte-encoded seed. It should be noted that this seed is not transferred.</param>
        /// <param name="security">Secuirty level of private key / seed.</param>
        /// <param name="index">
        ///     Key index to start search from. If the index is provided, the generation of the address is not
        ///     deterministic.
        /// </param>
        /// <returns>trytes</returns>
        public string GetDigest(string seed, int security, int index)
        {
            var key = _signing.Key(Converter.ToTrits(seed, 243), index, security);
            return Converter.ToTrytes(_signing.Digests(key));
        }

        /// <summary>
        ///     Gets the key value of a seed
        /// </summary>
        /// <param name="seed">Tryte-encoded seed. It should be noted that this seed is not transferred</param>
        /// <param name="index">
        ///     Key index to start search from. If the index is provided, the generation of the address is not
        ///     deterministic.
        /// </param>
        /// <param name="security">Secuirty level of private key / seed.</param>
        /// <returns>trytes</returns>
        public string GetKey(string seed, int index, int security)
        {
            return Converter.ToTrytes(
                _signing.Key(Converter.ToTrits(seed, 81 * security),
                    index, security));
        }

        /// <summary>
        /// </summary>
        /// <param name="digests">digest trytes</param>
        public void AddAddressDigest(string[] digests)
        {
            foreach (var digest in digests)
            {
                // Get trits of digest
                var digestTrits = Converter.ToTrits(digest);

                // Absorb digest
                _curl.Absorb(digestTrits, 0, digestTrits.Length);
            }
        }

        /// <summary>
        ///     Generates a new address
        /// </summary>
        /// <returns>address</returns>
        public string FinalizeAddress()
        {
            var addressTrits = new sbyte[243];
            _curl.Squeeze(addressTrits);

            // Convert trits into trytes and return the address
            return Converter.ToTrytes(addressTrits);
        }

        /// <summary>
        /// </summary>
        /// <param name="multisigAddress"></param>
        /// <param name="digests"></param>
        /// <returns></returns>
        public bool ValidateAddress(string multisigAddress, sbyte[][] digests)
        {
            // initialize Curl with the provided state
            _curl.Reset();

            foreach (var digest in digests) _curl.Absorb(digest);

            var addressTrits = new sbyte[243];
            _curl.Squeeze(addressTrits);

            // Convert trits into trytes and return the address
            return Converter.ToTrytes(addressTrits).Equals(multisigAddress, StringComparison.Ordinal);
        }

        /// <summary>
        /// </summary>
        /// <param name="bundleToSign"></param>
        /// <param name="inputAddress"></param>
        /// <param name="keyTrytes"></param>
        /// <returns></returns>
        public Bundle AddSignature(Bundle bundleToSign, string inputAddress, string keyTrytes)
        {
            // Get the security used for the private key
            // 1 security level = 2187 trytes
            var security = keyTrytes.Length / Constants.MESSAGE_LENGTH;

            // convert private key trytes into trits
            var key = Converter.ToTrits(keyTrytes);


            // First get the total number of already signed transactions
            // use that for the bundle hash calculation as well as knowing
            // where to add the signature
            var numSignedTxs = 0;


            for (var i = 0; i < bundleToSign.Transactions.Count; i++)
                if (bundleToSign.Transactions[i].Address.Equals(inputAddress, StringComparison.Ordinal))
                    if (!InputValidator.IsNinesTrytes(bundleToSign.Transactions[i].SignatureMessageFragment,
                        bundleToSign.Transactions[i].SignatureMessageFragment.Length))
                    {
                        numSignedTxs++;
                    }
                    // Else sign the transactions
                    else
                    {
                        var bundleHash = bundleToSign.Transactions[i].Bundle;

                        //  First 6561 trits for the firstFragment
                        var firstFragment = new sbyte[6561];
                        Array.Copy(key, firstFragment, 6561);

                        //  Get the normalized bundle hash
                        var normalizedBundleFragments = new sbyte[3][];
                        for (var n = 0; n < 3; n++) normalizedBundleFragments[n] = new sbyte[27];

                        var normalizedBundleHash = bundleToSign.NormalizedBundle(bundleHash);


                        // Split hash into 3 fragments
                        for (var k = 0; k < 3; k++)
                            Array.Copy(normalizedBundleHash, k * 27, normalizedBundleFragments[k], 0, 27);


                        //  First bundle fragment uses 27 trytes
                        var firstBundleFragment = normalizedBundleFragments[numSignedTxs % 3];

                        //  Calculate the new signatureFragment with the first bundle fragment
                        var firstSignedFragment = _signing.SignatureFragment(firstBundleFragment, firstFragment);

                        //  Convert signature to trytes and assign the new signatureFragment
                        bundleToSign.Transactions[i].SignatureMessageFragment = Converter.ToTrytes(firstSignedFragment);

                        for (var j = 1; j < security; j++)
                        {
                            //  Next 6561 trits for the firstFragment
                            var nextFragment = new sbyte[6561];
                            Array.Copy(key, 6561 * j, nextFragment, 0, 6561);

                            //  Use the next 27 trytes
                            var nextBundleFragment = normalizedBundleFragments[(numSignedTxs + j) % 3];

                            //  Calculate the new signatureFragment with the first bundle fragment
                            var nextSignedFragment = _signing.SignatureFragment(nextBundleFragment, nextFragment);

                            //  Convert signature to trytes and add new bundle entry at i + j position
                            // Assign the signature fragment
                            bundleToSign.Transactions[i + j].SignatureMessageFragment =
                                Converter.ToTrytes(nextSignedFragment);
                        }

                        break;
                    }

            return bundleToSign;
        }
    }
}
