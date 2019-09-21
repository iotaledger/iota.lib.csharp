using System;
using System.Collections.Generic;
using IotaSharp.Model;
using IotaSharp.Pow;

namespace IotaSharp.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class BundleValidator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="curl"></param>
        /// <returns></returns>
        public static bool IsBundle(Bundle bundle, ICurl curl = null)
        {
            if (curl == null)
                curl = SpongeFactory.Create(SpongeFactory.Mode.KERL);


            long totalSum = 0;
            int lastIndex = bundle.Length - 1;

            for (int i = 0; i < bundle.Length; i++)
            {
                var tx = bundle.Transactions[i];
                totalSum += tx.Value;

                if (tx.CurrentIndex != i)
                {
                    throw new ArgumentException(Constants.INVALID_BUNDLE_ERROR);
                }
                if (tx.LastIndex != lastIndex)
                {
                    throw new ArgumentException(Constants.INVALID_BUNDLE_ERROR);
                }

                sbyte[] txTrits = Converter.ToTrits(tx.ToTrytes().Substring(2187, 162));
                curl.Absorb(txTrits);

                // continue if output or signature tx
                if (tx.Value>= 0)
                {
                    continue;
                }

                // here we have an input transaction (negative value)
                List<string> fragments = new List<string> {tx.SignatureMessageFragment};

                // find the subsequent txs containing the remaining signature
                // message fragments for this input transaction
                for (int j = i; j < bundle.Length - 1; j++)
                {
                    Transaction tx2 = bundle.Transactions[j+1];

                    // check if the tx is part of the input transaction
                    if (tx.Address.Equals(tx2.Address, StringComparison.Ordinal) && tx2.Value == 0)
                    {
                        // append the signature message fragment
                        fragments.Add(tx2.SignatureMessageFragment);
                    }
                }

                bool valid = new Signing(curl.Clone()).ValidateSignatures(tx.Address, fragments.ToArray(), tx.Bundle);
                if (!valid)
                {
                    throw new ArgumentException(Constants.INVALID_SIGNATURES_ERROR);
                }
            }

            // sum of all transaction must be 0
            if (totalSum != 0)
            {
                throw new ArgumentException(Constants.INVALID_BUNDLE_SUM_ERROR);
            }

            sbyte[] bundleHashTrits = new sbyte[Sponge.HASH_LENGTH];
            curl.Squeeze(bundleHashTrits, 0, Sponge.HASH_LENGTH);
            string bundleHash = Converter.ToTrytes(bundleHashTrits);

            if (!bundleHash.Equals(bundle.Transactions[0].Bundle, StringComparison.Ordinal))
            {
                throw new ArgumentException(Constants.INVALID_BUNDLE_HASH_ERROR);
            }
            return true;
        }
    }
}
