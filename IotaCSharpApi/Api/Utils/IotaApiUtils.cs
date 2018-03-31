using System;
using System.Collections.Generic;
using Iota.Lib.CSharp.Api.Model;
using Iota.Lib.CSharp.Api.Pow;

namespace Iota.Lib.CSharp.Api.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class IotaApiUtils
    {
        /// <summary>
        ///  Generates a new address
        /// </summary>
        /// <param name="seed">The tryte-encoded seed. It should be noted that this seed is not transferred.</param>
        /// <param name="security">The secuirty level of private key / seed.</param>
        /// <param name="index">The index to start search from. If the index is provided, the generation of the address is not deterministic.</param>
        /// <param name="checksum">The adds 9-tryte address checksum</param>
        /// <param name="curl">The curl instance.</param>
        /// <returns>An String with address.</returns>
        public static string NewAddress(string seed, int security, int index, bool checksum, ICurl curl)
        {
            if (security < 1)
                throw new ArgumentException("invalid security level provided");

            Signing signing = new Signing(curl);
            int[] key = signing.Key(Converter.ToTrits(seed), index, security);
            int[] digests = signing.Digests(key);
            int[] addressTrits = signing.Address(digests);

            string address = Converter.ToTrytes(addressTrits);

            if (checksum)
                address = Checksum.AddChecksum(address);

            return address;
        }

        internal static List<string> SignInputsAndReturn(string seed,
            List<Input> inputs,
            Bundle bundle,
            List<string> signatureFragments, ICurl curl)
        {
            bundle.FinalizeBundle(curl);
            bundle.AddTrytes(signatureFragments);

            //  SIGNING OF INPUTS
            //
            //  Here we do the actual signing of the inputs
            //  Iterate over all bundle transactions, find the inputs
            //  Get the corresponding private key and calculate the signatureFragment
            for (int i = 0; i < bundle.Transactions.Count; i++)
            {
                if (bundle.Transactions[i].Value < 0)
                {
                    string thisAddress = bundle.Transactions[i].Address;

                    // Get the corresponding keyIndex of the address
                    int keyIndex = 0;
                    foreach (Input input in inputs)
                    {
                        if (input.Address.Equals(thisAddress))
                        {
                            keyIndex = input.KeyIndex;
                            break;
                        }
                    }

                    string bundleHash = bundle.Transactions[i].Bundle;

                    // Get corresponding private key of address
                    int[] key = new Signing(curl).Key(Converter.ToTrits(seed), keyIndex, 2);

                    //  First 6561 trits for the firstFragment
                    int[] firstFragment = ArrayUtils.SubArray2(key, 0, 6561);

                    //  Get the normalized bundle hash
                    int[] normalizedBundleHash = bundle.NormalizedBundle(bundleHash);

                    //  First bundle fragment uses 27 trytes
                    int[] firstBundleFragment = ArrayUtils.SubArray2(normalizedBundleHash, 0, 27);

                    //  Calculate the new signatureFragment with the first bundle fragment
                    int[] firstSignedFragment = new Signing(curl).SignatureFragment(firstBundleFragment, firstFragment);

                    //  Convert signature to trytes and assign the new signatureFragment
                    bundle.Transactions[i].SignatureMessageFragment = Converter.ToTrytes(firstSignedFragment);

                    //  Because the signature is > 2187 trytes, we need to
                    //  find the second transaction to add the remainder of the signature
                    for (int j = 0; j < bundle.Transactions.Count; j++)
                    {
                        //  Same address as well as value = 0 (as we already spent the input)
                        if (bundle.Transactions[j].Address.Equals(thisAddress) &&
                            bundle.Transactions[j].Value == 0)
                        {
                            // Use the second 6562 trits
                            int[] secondFragment = ArrayUtils.SubArray2(key, 6561, 6561);

                            // The second 27 to 54 trytes of the bundle hash
                            int[] secondBundleFragment = ArrayUtils.SubArray2(normalizedBundleHash, 27, 27);

                            //  Calculate the new signature
                            int[] secondSignedFragment = new Signing(curl).SignatureFragment(secondBundleFragment,
                                secondFragment);

                            //  Convert signature to trytes and assign it again to this bundle entry
                            bundle.Transactions[j].SignatureMessageFragment = (Converter.ToTrytes(secondSignedFragment));
                        }
                    }
                }
            }

            List<string> bundleTrytes = new List<string>();

            // Convert all bundle entries into trytes
            foreach (Transaction tx in bundle.Transactions)
            {
                bundleTrytes.Add(tx.ToTransactionTrytes());
            }

            bundleTrytes.Reverse();
            return bundleTrytes;
        }

        internal static long CreateTimeStampNow()
        {
            return (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }
    }
}