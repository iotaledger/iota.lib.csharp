using System;
using System.Collections.Generic;
using Iota.Lib.CSharp.Api.Utils;

namespace Iota.Lib.CSharp.Api.Model
{
    /// <summary>
    /// This class represents a Bundle, a set of transactions
    /// </summary>
    public class Bundle : IComparable<Bundle>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bundle"/> class without transactions.
        /// </summary>
        public Bundle() : this(new List<Transaction>(), 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bundle"/> class.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <param name="length">The length.</param>
        public Bundle(List<Transaction> transactions, int length)
        {
            Transactions = transactions;
            Length = length;
        }

        /// <summary>
        /// Gets the <see cref="Transaction"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="Transaction"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Transaction this[int index]
        {
            get { return Transactions[index]; }
        }

        /// <summary>
        /// Gets or sets the transactions.
        /// </summary>
        /// <value>
        /// The transactions.
        /// </value>
        public List<Transaction> Transactions { get; set; }

        /// <summary>
        /// Gets or sets the length of the bundle
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length { get; set; }

        /// <summary>
        /// Adds a bundle entry
        /// </summary>
        /// <param name="signatureMessageLength">Length of the signature message.</param>
        /// <param name="address">The address.</param>
        /// <param name="value">The value.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="timestamp">The timestamp.</param>
        public void AddEntry(int signatureMessageLength, string address, long value, string tag, long timestamp)
        {
            for (int i = 0; i < signatureMessageLength; i++)
            {
                Transaction trx = new Transaction(address, (i == 0 ? value : 0).ToString(), tag, timestamp.ToString());
                Transactions.Add(trx);
            }
        }

        /// <summary>
        /// Adds the trytes.
        /// </summary>
        /// <param name="signatureFragments">The signature fragments.</param>
        public void AddTrytes(List<string> signatureFragments)
        {
            string emptySignatureFragment = "";
            string emptyHash = Constants.EmptyHash;

            for (int j = 0; emptySignatureFragment.Length < 2187; j++)
            {
                emptySignatureFragment += '9';
            }

            for (int i = 0; i < Transactions.Count; i++)
            {
                Transaction transaction = Transactions[i];

                // Fill empty signatureMessageFragment
                transaction.SignatureFragment = ((signatureFragments.Count <= i ||
                                                  string.IsNullOrEmpty(signatureFragments[i]))
                    ? emptySignatureFragment
                    : signatureFragments[i]);
                // Fill empty trunkTransaction
                transaction.TrunkTransaction = emptyHash;

                // Fill empty branchTransaction
                transaction.BranchTransaction = emptyHash;

                // Fill empty nonce
                transaction.Nonce = emptyHash;
            }
        }


        /// <summary>
        /// Normalizeds the bundle.
        /// </summary>
        /// <param name="bundleHash">The bundle hash.</param>
        /// <returns></returns>
        public int[] NormalizedBundle(string bundleHash)
        {
            int[] normalizedBundle = new int[81];

            for (int i = 0; i < 3; i++)
            {
                long sum = 0;
                for (int j = 0; j < 27; j++)
                {
                    sum +=
                    (normalizedBundle[i*27 + j] =
                        Converter.ToValue(Converter.ToTritsString("" + bundleHash[i*27 + j])));
                }

                if (sum >= 0)
                {
                    while (sum-- > 0)
                    {
                        for (int j = 0; j < 27; j++)
                        {
                            if (normalizedBundle[i*27 + j] > -13)
                            {
                                normalizedBundle[i*27 + j]--;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    while (sum++ < 0)
                    {
                        for (int j = 0; j < 27; j++)
                        {
                            if (normalizedBundle[i*27 + j] < 13)
                            {
                                normalizedBundle[i*27 + j]++;
                                break;
                            }
                        }
                    }
                }
            }

            return normalizedBundle;
        }


        /// <summary>
        /// Finalizes the bundle using the specified curl implementation
        /// </summary>
        /// <param name="customCurl">The custom curl.</param>
        public void FinalizeBundle(ICurl customCurl)
        {
            customCurl.Reset();

            for (int i = 0; i < Transactions.Count; i++)
            {
                int[] valueTrits = Converter.ToTrits(Transactions[i].Value, 81);

                int[] timestampTrits = Converter.ToTrits(Transactions[i].Timestamp, 27);

                int[] currentIndexTrits = Converter.ToTrits(Transactions[i].CurrentIndex = ("" + i), 27);

                int[] lastIndexTrits = Converter.ToTrits(
                    Transactions[i].LastIndex = ("" + (this.Transactions.Count - 1)), 27);

                string stringToConvert = Transactions[i].Address
                                         + Converter.ToTrytes(valueTrits)
                                         + Transactions[i].Tag +
                                         Converter.ToTrytes(timestampTrits)
                                         + Converter.ToTrytes(currentIndexTrits) +
                                         Converter.ToTrytes(lastIndexTrits);

                int[] t = Converter.ToTrits(stringToConvert);
                customCurl.Absorb(t, 0, t.Length);
            }

            int[] hash = new int[243];
            customCurl.Squeeze(hash, 0, hash.Length);
            string hashInTrytes = Converter.ToTrytes(hash);

            for (int i = 0; i < Transactions.Count; i++)
            {
                Transactions[i].Bundle = hashInTrytes;
            }
        }

        /// <summary>
        /// Finalizes the bundle.
        /// </summary>
        public void FinalizeBundle()
        {
            FinalizeBundle(new Curl());
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(Bundle other)
        {
            long timeStamp1 = long.Parse(Transactions[0].Timestamp);
            long timeStamp2 = long.Parse(other.Transactions[0].Timestamp);

            if (timeStamp1 < timeStamp2)
                return -1;
            if (timeStamp1 > timeStamp2)
                return 1;
            return 0;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Transactions)}: {string.Join(",", Transactions)}";
        }
    }
}