using System.Collections.Generic;
using System.Text.RegularExpressions;
using IotaSharp.Core;
using IotaSharp.Exception;
using IotaSharp.Model;
using IotaSharp.Utils;

namespace IotaSharp.Pow
{
    /// <summary>
    /// 
    /// </summary>
    public class PearlDiverLocalPoW
    {
        private readonly PearlDiver _pearlDiver = new PearlDiver();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trunkTransaction"></param>
        /// <param name="branchTransaction"></param>
        /// <param name="minWeightMagnitude"></param>
        /// <param name="trytes"></param>
        /// <returns></returns>
        public AttachToTangleResponse AttachToTangle(
            string trunkTransaction, string branchTransaction,
            int minWeightMagnitude, string[] trytes)
        {
            var response = new AttachToTangleResponse
            {
                Trytes = new List<string>()
            };

            string previousTransaction = null;
            foreach (var t in trytes)
            {
                var txn = new Transaction(t)
                {
                    TrunkTransaction = previousTransaction ?? trunkTransaction,
                    BranchTransaction = previousTransaction == null ? branchTransaction : trunkTransaction
                };

                if (string.IsNullOrEmpty(txn.Tag) || Regex.IsMatch(txn.Tag, "9*"))
                    txn.Tag = txn.ObsoleteTag;

                txn.AttachmentTimestamp = TimeStamp.Now();
                txn.AttachmentTimestampLowerBound = 0;
                txn.AttachmentTimestampUpperBound = 3_812_798_742_493L;

                // POW
                var transactionTrits = Converter.ToTrits(txn.ToTrytes());
                if (!_pearlDiver.Search(transactionTrits, minWeightMagnitude, 0))
                    throw new IllegalStateException("PearlDiver search failed");

                // Hash
                var hash = new sbyte[Sponge.HASH_LENGTH];

                ICurl curl = SpongeFactory.Create(SpongeFactory.Mode.CURLP81);
                curl.Reset();
                curl.Absorb(transactionTrits);
                curl.Squeeze(hash);

                previousTransaction = Converter.ToTrytes(hash);

                response.Trytes.Add(Converter.ToTrytes(transactionTrits));
            }

            response.Trytes.Reverse();
            return response;
        }
    }
}
