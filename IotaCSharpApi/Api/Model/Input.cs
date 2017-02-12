namespace Iota.Lib.CSharp.Api.Model
{
    /// <summary>
    /// This class represents an Input
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        /// <value>
        /// The balance.
        /// </value>
        public long Balance { get; set; }

        /// <summary>
        /// Gets or sets the index of the key.
        /// </summary>
        /// <value>
        /// The index of the key.
        /// </value>
        public int KeyIndex { get; set; }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Address)}: {Address}, {nameof(Balance)}: {Balance}, {nameof(KeyIndex)}: {KeyIndex}";
        }
    }
}