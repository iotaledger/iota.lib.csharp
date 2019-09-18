using System;
using System.Collections.Generic;
using IotaSharp.Model;
using IotaSharp.Pow;

namespace IotaSharp.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class IotaApiUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="security"></param>
        /// <param name="index"></param>
        /// <param name="checksum"></param>
        /// <param name="curl"></param>
        /// <returns></returns>
        public static string NewAddress(string seed, int security, int index, bool checksum, ICurl curl)
        {
            if (!InputValidator.IsValidSecurityLevel(security))
                throw new ArgumentException(Constants.INVALID_SECURITY_LEVEL_INPUT_ERROR);

            Signing signing = new Signing(curl);
            sbyte[] key = signing.Key(Converter.ToTrits(seed), index, security);
            sbyte[] digests = signing.Digests(key);
            sbyte[] addressTrits = signing.Address(digests);

            string address = Converter.ToTrytes(addressTrits);

            if (checksum)
                address = address.AddChecksum();

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
                    int keySecurity = 0;
                    foreach (Input input in inputs)
                    {
                        if (input.Address.StartsWith(thisAddress, StringComparison.Ordinal))
                        {
                            keyIndex = input.KeyIndex;
                            keySecurity = input.Security;
                            break;
                        }
                    }

                    string bundleHash = bundle.Transactions[i].Bundle;

                    // Get corresponding private key of address
                    sbyte[] key = new Signing(curl).Key(Converter.ToTrits(seed), keyIndex, keySecurity);

                    //  First 6561 trits for the firstFragment
                    //sbyte[] firstFragment = ArrayUtils.SubArray2(key, 0, 6561);

                    //  Get the normalized bundle hash
                    sbyte[] normalizedBundleHash = bundle.NormalizedBundle(bundleHash);

                    //  First bundle fragment uses 27 trytes
                    //sbyte[] firstBundleFragment = ArrayUtils.SubArray2(normalizedBundleHash, 0, 27);

                    //  Calculate the new signatureFragment with the first bundle fragment
                    //sbyte[] firstSignedFragment =
                    //    new Signing(curl).SignatureFragment(firstBundleFragment, firstFragment);

                    //  Convert signature to trytes and assign the new signatureFragment
                    //bundle.Transactions[i].SignatureMessageFragment = Converter.ToTrytes(firstSignedFragment);


                    // if user chooses higher than 27-tryte security
                    // for each security level, add an additional signature
                    for (int j = 0; j < keySecurity; j++)
                    {
                        int hashPart = j % 3;
                        //  Add parts of signature for bundles with same address
                        if (bundle.Transactions[i + j].Address.StartsWith(thisAddress, StringComparison.Ordinal))
                        {
                            // Use 6562 trits starting from j*6561
                            sbyte[] keyFragment = ArrayUtils.SubArray2(key, 6561 * j, 6561);

                            // The current part of the bundle hash
                            sbyte[] bundleFragment =
                                ArrayUtils.SubArray2(normalizedBundleHash, 27 * hashPart, 27);

                            //  Calculate the new signature
                            sbyte[] signedFragment = new Signing(curl).SignatureFragment(bundleFragment,
                                keyFragment);

                            //  Convert signature to trytes and assign it again to this bundle entry
                            bundle.Transactions[i + j].SignatureMessageFragment =
                                (Converter.ToTrytes(signedFragment));
                        }
                    }

                }
            }

            List<string> bundleTrytes = new List<string>();

            // Convert all bundle entries into trytes
            foreach (Transaction tx in bundle.Transactions)
            {
                bundleTrytes.Add(tx.ToTrytes());
            }

            bundleTrytes.Reverse();
            return bundleTrytes;
        }
    }
}
