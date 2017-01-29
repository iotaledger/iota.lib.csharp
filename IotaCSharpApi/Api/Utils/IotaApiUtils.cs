using System;
using System.Collections.Generic;
using System.Diagnostics;
using Iota.Lib.CSharp.Api.Model;

namespace Iota.Lib.CSharp.Api.Utils
{
    public class IotaApiUtils
    {
        /// <summary>
        ///  Generates a new address
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="index"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        public static string NewAddress(string seed, int index, bool checksum, ICurl curl)
        {
            Signing signing = new Signing(curl);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds/10);
            Console.WriteLine(elapsedTime);

            int[] key = signing.Key(Converter.trits(seed), index, 2);

            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds/10);
            Console.WriteLine(elapsedTime);

            int[] digests = signing.Digests(key);

            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("After Digest {0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds/10);
            Console.WriteLine(elapsedTime);

            int[] addressTrits = signing.Address(digests);
            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds/10);
            Console.WriteLine(elapsedTime);
            string address = Converter.trytes(addressTrits);

            if (checksum)
                address = address.AddChecksum();

            return address;
        }

        /**
             * Generates a new address
             *
             * @param seed
             * @param index
             * @param checksum
             * @return an String with address
             */


        public static List<string> signInputsAndReturn(string seed,
            List<Input> inputs,
            Bundle bundle,
            List<string> signatureFragments, ICurl curl)
        {
            bundle.finalize(curl);
            bundle.addTrytes(signatureFragments);

            //  SIGNING OF INPUTS
            //
            //  Here we do the actual signing of the inputs
            //  Iterate over all bundle transactions, find the inputs
            //  Get the corresponding private key and calculate the signatureFragment
            for (int i = 0; i < bundle.Transactions.Count; i++)
            {
                if (long.Parse(bundle.Transactions[i].Value) < 0)
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
                    int[] key = new Signing(curl).Key(Converter.trits(seed), keyIndex, 2);

                    //  First 6561 trits for the firstFragment
                    int[] firstFragment = SubArray(key, 0, 6561);

                    //  Get the normalized bundle hash
                    int[] normalizedBundleHash = bundle.normalizedBundle(bundleHash);

                    //  First bundle fragment uses 27 trytes
                    int[] firstBundleFragment = SubArray(normalizedBundleHash, 0, 27);

                    //  Calculate the new signatureFragment with the first bundle fragment
                    int[] firstSignedFragment = new Signing(curl).signatureFragment(firstBundleFragment, firstFragment);

                    //  Convert signature to trytes and assign the new signatureFragment
                    bundle.Transactions[i].SignatureFragment = Converter.trytes(firstSignedFragment);

                    //  Because the signature is > 2187 trytes, we need to
                    //  find the second transaction to add the remainder of the signature
                    for (int j = 0; j < bundle.Transactions.Count; j++)
                    {
                        //  Same address as well as value = 0 (as we already spent the input)
                        if (bundle.Transactions[j].Address.Equals(thisAddress) &&
                            long.Parse(bundle.Transactions[j].Value) == 0)
                        {
                            // Use the second 6562 trits
                            int[] secondFragment = SubArray(key, 6561, 6561);

                            // The second 27 to 54 trytes of the bundle hash
                            int[] secondBundleFragment = SubArray(normalizedBundleHash, 27, 27);

                            //  Calculate the new signature
                            int[] secondSignedFragment = new Signing(curl).signatureFragment(secondBundleFragment,
                                secondFragment);

                            //  Convert signature to trytes and assign it again to this bundle entry
                            bundle.Transactions[j].SignatureFragment = (Converter.trytes(secondSignedFragment));
                        }
                    }
                }
            }

            List<string> bundleTrytes = new List<string>();

            // Convert all bundle entries into trytes
            foreach (Transaction tx in bundle.Transactions)
            {
                bundleTrytes.Add(Converter.transactionTrytes(tx));
            }
            bundleTrytes.Reverse();
            return bundleTrytes;
        }

        public static T[] SubArray<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}