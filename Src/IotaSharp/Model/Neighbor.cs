namespace IotaSharp.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class Neighbor
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the number of all transactions.
        /// </summary>
        /// <value>
        /// The number of all transactions.
        /// </value>
        public long NumberOfAllTransactions { get; set; }

        /// <summary>
        /// Gets or sets the number of invalid transactions.
        /// </summary>
        /// <value>
        /// The number of invalid transactions.
        /// </value>
        public long NumberOfInvalidTransactions { get; set; }

        /// <summary>
        /// Gets or sets the number of new transactions.
        /// </summary>
        /// <value>
        /// The number of new transactions.
        /// </value>
        public long NumberOfNewTransactions { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return
                $"{nameof(Address)}: {Address}, {nameof(NumberOfAllTransactions)}: {NumberOfAllTransactions}, {nameof(NumberOfInvalidTransactions)}: {NumberOfInvalidTransactions}, {nameof(NumberOfNewTransactions)}: {NumberOfNewTransactions}";
        }
    }
}
