using System;
using Iota.Lib.CSharp.Api.Utils;

namespace Iota.Lib.CSharp.Api.Model
{
    /// <summary>
    /// This class represents an iota transaction
    /// </summary>
    public class Transaction
    {
        public Transaction(string address, string value, string tag, string timestamp)
        {
            this.Address = address;
            this.Value = value;
            this.Tag = tag;
            this.Timestamp = timestamp;
        }

        public Transaction(string trytes, ICurl curl)
        {
            if (String.IsNullOrEmpty(trytes))
            {
                throw new ArgumentException("trytes must non-null");
            }

            // validity check
            for (int i = 2279; i < 2295; i++)
            {
                if (trytes[i] != '9')
                {
                    throw new ArgumentException("position " + i + "must not be '9'");
                }
            }

            int[] transactionTrits = Converter.ToTrits(trytes);
            int[] hash = new int[243];

            // generate the correct transaction hash
            curl.Reset()
                .Absorb(transactionTrits, 0, transactionTrits.Length)
                .Squeeze(hash, 0, hash.Length);

            Hash = Converter.ToTrytes(hash);
            SignatureFragment = trytes.Substring(0, 2187);
            Address = trytes.Substring(2187, 2268 - 2187);
            Value = "" + Converter.ToLongValue(SubArray(transactionTrits, 6804, 6837));
            Tag = trytes.Substring(2295, 2322 - 2295);
            Timestamp = "" + Converter.ToLongValue(SubArray(transactionTrits, 6966, 6993));
            CurrentIndex = "" + Converter.ToLongValue(SubArray(transactionTrits, 6993, 7020));
            LastIndex = "" + Converter.ToLongValue(SubArray(transactionTrits, 7020, 7047));
            Bundle = trytes.Substring(2349, 2430 - 2349);
            TrunkTransaction = trytes.Substring(2430, 2511 - 2430);
            BranchTransaction = trytes.Substring(2511, 2592 - 2511);
            Nonce = trytes.Substring(2592, 2673 - 2592);
        }

        public Transaction()
        {
        }



        public string Tag { get; set; }

        /// <returns> The hash </returns>
        public string Hash { get; set; }

        /// <returns> The type </returns>
        public string Type { get; set; }

        /// <returns> The signatureMessageChunk </returns>
        public string SignatureMessageChunk { get; set; }

        /// <returns> The signatureMessageChunk </returns>
        public string Digest { get; set; }

        /// <returns> The address </returns>
        public string Address { get; set; }

        /// <returns> The value </returns>
        public string Value { get; set; }

        /// <returns> The timestamp </returns>
        public string Timestamp { get; set; }

        /// <returns> The bundle </returns>
        public string Bundle { get; set; }

        /// <returns> The index </returns>
        public int Index { get; set; }

        /// <returns> The transaction </returns>
        public string TrunkTransaction { get; set; }

        /// <returns> The branchTransaction </returns>
        public string BranchTransaction { get; set; }

        public string SignatureFragment { get; set; }

        public string LastIndex { get; set; }

        public string CurrentIndex { get; set; }

        public string Nonce { get; set; }

        public bool Persistance { get; set; }

        public string ToTransactionTrytes()
        {
            int[] valueTrits = Converter.ToTrits(Value, 81);
            int[] timestampTrits = Converter.ToTrits(Timestamp, 27);
            int[] currentIndexTrits = Converter.ToTrits(CurrentIndex, 27);
            int[] lastIndexTrits = Converter.ToTrits(LastIndex, 27);

            return SignatureFragment
                   + Address
                   + Converter.ToTrytes(valueTrits)
                   + Tag
                   + Converter.ToTrytes(timestampTrits)
                   + Converter.ToTrytes(currentIndexTrits)
                   + Converter.ToTrytes(lastIndexTrits)
                   + Bundle
                   + TrunkTransaction
                   + BranchTransaction
                   + Nonce;
        }

        public static T[] SubArray<T>(T[] data, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            T[] result = new T[endIndex - startIndex];
            Array.Copy(data, startIndex, result, 0, length);
            return result;
        }
    }
}