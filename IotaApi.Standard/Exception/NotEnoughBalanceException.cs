namespace Iota.Api.Standard.Exception
{
    /// <summary>
    /// This exception occurs when a transfer fails because their is not enough balance to perform the transfer
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class NotEnoughBalanceException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotEnoughBalanceException"/> class.
        /// </summary>
        public NotEnoughBalanceException() : base("Not enough balance")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEnoughBalanceException"/> class.
        /// </summary>
        /// <param name="totalValue">The total value.</param>
        public NotEnoughBalanceException(long totalValue) : base("Not enough balance to transfer " + totalValue + " iota")
        {

        }
    }
}