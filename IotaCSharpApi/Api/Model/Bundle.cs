using System;
using System.Collections.Generic;
using Iota.Lib.CSharp.Api.Utils;

namespace Iota.Lib.CSharp.Api.Model
{
    public class Bundle : IComparable, IComparable<Bundle>
    {
        public List<Transaction> Transactions { get; set; }

        public int Length { get; set; }


        public Bundle() : this(new List<Transaction>(), 0)
        {
        }

        public Bundle(List<Transaction> transactions, int length)
        {
            this.Transactions = transactions;
            this.Length = length;
        }


        public Transaction this[int index]
        {
            get { return Transactions[index]; }
        }

        public void addEntry(int signatureMessageLength, string address, long value, string tag, long timestamp)
        {
            /*if (Transactions == null)
            {
                this.Transactions = new List<Transaction>(Transactions);
            }*/

            for (int i = 0; i < signatureMessageLength; i++)
            {
                Transaction trx = new Transaction(address, (i == 0 ? value : 0).ToString(), tag, timestamp.ToString());
                Transactions.Add(trx);
            }
        }

        public void finalize(ICurl customCurl)
        {
            ICurl curl = customCurl == null ? new Curl() : customCurl;
            curl.Reset();

            for (int i = 0; i < Transactions.Count; i++)
            {
                int[] valueTrits = Converter.trits(Transactions[i].Value, 81);

                int[] timestampTrits = Converter.trits(Transactions[i].Timestamp, 27);

                int[] currentIndexTrits = Converter.trits(Transactions[i].CurrentIndex = ("" + i), 27);

                int[] lastIndexTrits = Converter.trits(
                    Transactions[i].LastIndex = ("" + (this.Transactions.Count - 1)), 27);

                string stringToConvert = Transactions[i].Address
                                         + Converter.trytes(valueTrits)
                                         + Transactions[i].Tag +
                                         Converter.trytes(timestampTrits)
                                         + Converter.trytes(currentIndexTrits) +
                                         Converter.trytes(lastIndexTrits);

                int[] t = Converter.trits(stringToConvert);
                curl.Absorb(t, 0, t.Length);
            }

            int[] hash = new int[243];
            curl.Squeeze(hash, 0, hash.Length);
            String hashInTrytes = Converter.trytes(hash);

            for (int i = 0; i < Transactions.Count; i++)
            {
                Transactions[i].Bundle = hashInTrytes;
            }
        }

        public void finalize()
        {
            finalize(new Curl());
        }

        public void addTrytes(List<string> signatureFragments)
        {
            String emptySignatureFragment = "";
            String emptyHash = Constants.EMPTY_HASH;

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

        public int[] normalizedBundle(string bundleHash)
        {
            int[] normalizedBundle = new int[81];

            for (int i = 0; i < 3; i++)
            {
                long sum = 0;
                for (int j = 0; j < 27; j++)
                {
                    sum +=
                        (normalizedBundle[i*27 + j] = Converter.value(Converter.tritsString("" + bundleHash[i*27 + j])));
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

        public int CompareTo(object obj)
        {
            long timeStamp1 = long.Parse(Transactions[0].Timestamp);
            long timeStamp2 = long.Parse(((Bundle) obj).Transactions[0].Timestamp);

            if (timeStamp1 < timeStamp2)
                return -1;
            if (timeStamp1 > timeStamp2)
                return 1;
            return 0;
        }

        public int CompareTo(Bundle other)
        {
            return CompareTo(other);
        }
    }
}