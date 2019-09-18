using System;
using System.Collections.Generic;
using IotaSharp.Pow;
using IotaSharp.Utils;

namespace IotaSharp.Model
{
    /// <summary>
    ///     This class represents a Bundle, a set of transactions
    /// </summary>
    public class Bundle : IComparable<Bundle>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Bundle" /> class without transactions.
        /// </summary>
        public Bundle() : this(new List<Transaction>(), 0)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Bundle" /> class.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <param name="length">The length.</param>
        public Bundle(List<Transaction> transactions, int length)
        {
            Transactions = transactions;
            Length = length;
        }

        /// <summary>
        ///     Gets the <see cref="Transaction" /> at the specified index.
        /// </summary>
        /// <value>
        ///     The <see cref="Transaction" />.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Transaction this[int index] => Transactions[index];

        /// <summary>
        ///     Gets or sets the transactions.
        /// </summary>
        /// <value>
        ///     The transactions.
        /// </value>
        public List<Transaction> Transactions { get; private set; }

        /// <summary>
        ///     Gets or sets the length of the bundle
        /// </summary>
        /// <value>
        ///     The length.
        /// </value>
        public int Length { get; set; }

        /// <summary>
        ///     Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
        ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        public int CompareTo(Bundle other)
        {
            var timeStamp1 = Transactions[0].Timestamp;
            var timeStamp2 = other.Transactions[0].Timestamp;

            if (timeStamp1 < timeStamp2)
                return -1;
            if (timeStamp1 > timeStamp2)
                return 1;
            return 0;
        }

        /// <summary>
        ///     Adds a bundle entry
        /// </summary>
        /// <param name="signatureMessageLength">Length of the signature message.</param>
        /// <param name="address">The address.</param>
        /// <param name="value">The value.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="timestamp">The timestamp.</param>
        public void AddEntry(int signatureMessageLength, string address, long value, string tag, long timestamp)
        {
            if (Transactions == null)
                Transactions = new List<Transaction>();

            for (var i = 0; i < signatureMessageLength; i++)
            {
                var trx = new Transaction(address, i == 0 ? value : 0, tag, timestamp);
                Transactions.Add(trx);
            }
        }

        /// <summary>
        ///     Adds the trytes.
        /// </summary>
        /// <param name="signatureFragments">The signature fragments.</param>
        public void AddTrytes(List<string> signatureFragments)
        {
            var emptySignatureFragment = "";
            var emptyHash = Constants.NULL_HASH;
            long emptyTimestamp = 999999999L;

            while (emptySignatureFragment.Length < 2187) emptySignatureFragment += '9';

            for (var i = 0; i < Transactions.Count; i++)
            {
                var transaction = Transactions[i];

                // Fill empty signatureMessageFragment
                transaction.SignatureMessageFragment = signatureFragments.Count <= i ||
                                                       string.IsNullOrEmpty(signatureFragments[i])
                    ? emptySignatureFragment
                    : signatureFragments[i];
                // Fill empty trunkTransaction
                transaction.TrunkTransaction = emptyHash;

                // Fill empty branchTransaction
                transaction.BranchTransaction = emptyHash;

                transaction.AttachmentTimestamp = emptyTimestamp;
                transaction.AttachmentTimestampLowerBound = emptyTimestamp;
                transaction.AttachmentTimestampUpperBound = emptyTimestamp;
                // Fill empty nonce
                transaction.Nonce = "999999999999999999999999999";
            }
        }


        /// <summary>
        ///     Normalizeds the bundle.
        /// </summary>
        /// <param name="bundleHash">The bundle hash.</param>
        /// <returns></returns>
        public sbyte[] NormalizedBundle(string bundleHash)
        {
            var normalizedBundle = new sbyte[81];

            for (var i = 0; i < 3; i++)
            {
                long sum = 0;
                for (var j = 0; j < 27; j++)
                    sum +=
                        normalizedBundle[i * 27 + j] =
                            (sbyte) Converter.ToInt32(Converter.ToTrits("" + bundleHash[i * 27 + j]));

                if (sum >= 0)
                {
                    while (sum-- > 0)
                        for (var j = 0; j < 27; j++)
                            if (normalizedBundle[i * 27 + j] > -13)
                            {
                                normalizedBundle[i * 27 + j]--;
                                break;
                            }
                }
                else
                {
                    while (sum++ < 0)
                        for (var j = 0; j < 27; j++)
                            if (normalizedBundle[i * 27 + j] < 13)
                            {
                                normalizedBundle[i * 27 + j]++;
                                break;
                            }
                }
            }

            return normalizedBundle;
        }


        /// <summary>
        ///     Finalizes the bundle using the specified curl implementation
        /// </summary>
        /// <param name="customCurl">The custom curl.</param>
        public void FinalizeBundle(ICurl customCurl)
        {
            string hashInTrytes = string.Empty;

            bool validBundle = false;

            while (!validBundle)
            {
                customCurl.Reset();

                for (var i = 0; i < Transactions.Count; i++)
                {
                    var valueTrits = Converter.ToTrits(Transactions[i].Value, 81);

                    var timestampTrits = Converter.ToTrits(Transactions[i].Timestamp, 27);

                    var currentIndexTrits = Converter.ToTrits(Transactions[i].CurrentIndex = i, 27);

                    var lastIndexTrits = Converter.ToTrits(
                        Transactions[i].LastIndex = Transactions.Count - 1, 27);

                    var stringToConvert = Transactions[i].Address
                                          + Converter.ToTrytes(valueTrits)
                                          + Transactions[i].ObsoleteTag +
                                          Converter.ToTrytes(timestampTrits)
                                          + Converter.ToTrytes(currentIndexTrits) +
                                          Converter.ToTrytes(lastIndexTrits);

                    var t = Converter.ToTrits(stringToConvert);
                    customCurl.Absorb(t, 0, t.Length);
                }

                var hash = new sbyte[Sponge.HASH_LENGTH];
                customCurl.Squeeze(hash, 0, hash.Length);
                hashInTrytes = Converter.ToTrytes(hash);

                bool foundValue = false;
                var normalizedHash = NormalizedBundle(hashInTrytes);
                foreach (var normalizedHashValue in normalizedHash)
                {
                    if (normalizedHashValue == 13 /* = M */)
                    {
                        foundValue = true;
                        // Insecure bundle. Increment Tag and recompute bundle hash.
                        var obsoleteTagTrits = Converter.ToTrits(Transactions[0].ObsoleteTag);
                        Converter.Increment(obsoleteTagTrits, 81);
                        Transactions[0].ObsoleteTag = Converter.ToTrytes(obsoleteTagTrits);
                        break;
                    }
                }

                validBundle = !foundValue;
            }

            foreach (var transaction in Transactions) transaction.Bundle = hashInTrytes;
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Transactions)}: {string.Join(",", Transactions)}";
        }
    }
}
