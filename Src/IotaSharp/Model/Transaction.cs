using System;
using IotaSharp.Pow;
using IotaSharp.Utils;

namespace IotaSharp.Model
{
    /// <summary>
    ///     This class represents an iota transaction
    /// </summary>
    public class Transaction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Transaction" /> class.
        /// </summary>
        /// <param name="trytes">The trytes representing the transaction</param>
        public Transaction(string trytes)
        {
            TransactionObject(trytes);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Transaction" /> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="value">The value.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="timestamp">The timestamp.</param>
        public Transaction(string address, long value, string tag, long timestamp)
        {
            Address = address;
            Value = value;
            Tag = tag;
            ObsoleteTag = tag;
            Timestamp = timestamp;
        }

        /// <summary>
        ///     Gets or sets the signature fragment.
        /// </summary>
        /// <value>
        ///     The signature fragment.
        /// </value>
        public string SignatureMessageFragment { get; set; }


        private string _address;

        /// <summary>
        ///     Gets or sets the address.
        /// </summary>
        /// <value>
        ///     The address.
        /// </value>
        public string Address
        {
            get => _address;
            set => _address = value.RemoveChecksum();
        }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public long Value { get; set; }

        /// <summary>
        ///     Gets or sets the tag.
        /// </summary>
        /// <value>
        ///     The tag.
        /// </value>
        public string ObsoleteTag { get; set; }

        /// <summary>
        ///     Gets or sets the timestamp.
        /// </summary>
        /// <value>
        ///     The timestamp.
        /// </value>
        public long Timestamp { get; set; }

        /// <summary>
        ///     Gets or sets the index of the current.
        /// </summary>
        /// <value>
        ///     The index of the current.
        /// </value>
        public long CurrentIndex { get; set; }

        /// <summary>
        ///     Gets or sets the last index.
        /// </summary>
        /// <value>
        ///     The last index.
        /// </value>
        public long LastIndex { get; set; }

        /// <summary>
        ///     Gets or sets the bundle.
        /// </summary>
        /// <value>
        ///     The bundle.
        /// </value>
        public string Bundle { get; set; }

        /// <summary>
        ///     Gets or sets the trunk transaction.
        /// </summary>
        /// <value>
        ///     The trunk transaction.
        /// </value>
        public string TrunkTransaction { get; set; }

        /// <summary>
        ///     Gets or sets the branch transaction.
        /// </summary>
        /// <value>
        ///     The branch transaction.
        /// </value>
        public string BranchTransaction { get; set; }

        /// <summary>
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// </summary>
        public long AttachmentTimestamp { get; set; }

        /// <summary>
        /// </summary>
        public long AttachmentTimestampLowerBound { get; set; }

        /// <summary>
        /// </summary>
        public long AttachmentTimestampUpperBound { get; set; }

        /// <summary>
        ///     Gets or sets the nonce.
        /// </summary>
        /// <value>
        ///     The nonce.
        /// </value>
        public string Nonce { get; set; }

        public bool Persistence { get; set; }

        /// <summary>
        ///     Converts the transaction to the corresponding trytes representation
        /// </summary>
        /// <returns></returns>
        public string ToTrytes()
        {
            var valueTrits = Converter.ToTrits(Value, 81);
            var timestampTrits = Converter.ToTrits(Timestamp, 27);
            var currentIndexTrits = Converter.ToTrits(CurrentIndex, 27);
            var lastIndexTrits = Converter.ToTrits(LastIndex, 27);
            var attachmentTimestampTrits = Converter.ToTrits(AttachmentTimestamp, 27);
            var attachmentTimestampLowerBoundTrits = Converter.ToTrits(AttachmentTimestampLowerBound, 27);
            var attachmentTimestampUpperBoundTrits = Converter.ToTrits(AttachmentTimestampUpperBound, 27);

            if (string.IsNullOrEmpty(Tag))
                Tag = ObsoleteTag;

            return SignatureMessageFragment
                   + Address
                   + Converter.ToTrytes(valueTrits)
                   + ObsoleteTag
                   + Converter.ToTrytes(timestampTrits)
                   + Converter.ToTrytes(currentIndexTrits)
                   + Converter.ToTrytes(lastIndexTrits)
                   + Bundle
                   + TrunkTransaction
                   + BranchTransaction
                   + Tag
                   + Converter.ToTrytes(attachmentTimestampTrits)
                   + Converter.ToTrytes(attachmentTimestampLowerBoundTrits)
                   + Converter.ToTrytes(attachmentTimestampUpperBoundTrits)
                   + Nonce;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string CurlHash()
        {
            var transactionTrits = Converter.ToTrits(ToTrytes());
            var hash = new sbyte[Sponge.HASH_LENGTH];

            ICurl curl = SpongeFactory.Create(SpongeFactory.Mode.CURLP81);
            curl.Reset();
            curl.Absorb(transactionTrits);
            curl.Squeeze(hash);

            return Converter.ToTrytes(hash);
        }

        private void TransactionObject(string trytes)
        {
            if (string.IsNullOrEmpty(trytes)) throw new ArgumentException("trytes must non-null");

            // validity check
            for (var i = 2279; i < 2295; i++)
                if (trytes[i] != '9')
                    throw new ArgumentException("position " + i + "must not be '9'");

            var transactionTrits = Converter.ToTrits(trytes);

            SignatureMessageFragment = trytes.Substring(0, 2187);
            Address = trytes.Substring(2187, 2268 - 2187);
            Value = Converter.ToInt64(transactionTrits, 6804, 6837 - 6804);
            ObsoleteTag = trytes.Substring(2295, 2322 - 2295);
            Timestamp = Converter.ToInt64(transactionTrits, 6966, 6993 - 6966);
            CurrentIndex = Converter.ToInt64(transactionTrits, 6993, 7020 - 6993);
            LastIndex = Converter.ToInt64(transactionTrits, 7020, 7047 - 7020);
            Bundle = trytes.Substring(2349, 2430 - 2349);
            TrunkTransaction = trytes.Substring(2430, 2511 - 2430);
            BranchTransaction = trytes.Substring(2511, 2592 - 2511);
            Tag = trytes.Substring(2592, 2619 - 2592);
            AttachmentTimestamp = Converter.ToInt64(transactionTrits, 7857, 7884 - 7857);
            AttachmentTimestampLowerBound = Converter.ToInt64(transactionTrits, 7884, 7911 - 7884);
            AttachmentTimestampUpperBound = Converter.ToInt64(transactionTrits, 7911, 7938 - 7911);
            Nonce = trytes.Substring(2646, 2673 - 2646);
        }
    }
}
