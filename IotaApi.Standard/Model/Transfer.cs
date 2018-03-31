namespace Iota.Api.Standard.Model
{
    /// <summary>
    /// This class represents a Transfer
    /// </summary>
    public class Transfer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Transfer"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        public Transfer(string address, long value, string message, string tag)
        {
            Address = address;
            Value = value;
            Message = message;
            Tag = tag;
        }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>
        /// The hash.
        /// </value>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the persistence.
        /// </summary>
        /// <value>
        /// The persistence.
        /// </value>
        public int Persistence { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public string Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public long Value { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public string Tag { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Address)}: {Address}, {nameof(Hash)}: {Hash}, {nameof(Message)}: {Message}, {nameof(Persistence)}: {Persistence}, {nameof(Tag)}: {Tag}, {nameof(Timestamp)}: {Timestamp}, {nameof(Value)}: {Value}";
        }
    }
}