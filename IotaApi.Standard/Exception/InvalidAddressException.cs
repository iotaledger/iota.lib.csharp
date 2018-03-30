using System;

namespace Iota.Api.Standard.Exception
{
    /// <summary>
    /// This exception occurs when an invalid address is provided
    /// </summary>
    /// <seealso cref="ArgumentException" />
    public class InvalidAddressException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAddressException"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        public InvalidAddressException(string address) : base("The specified address '" + address + "' is invalid")
        {
        }
    }
}