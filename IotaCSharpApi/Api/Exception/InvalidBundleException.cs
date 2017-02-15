namespace Iota.Lib.CSharp.Api.Exception
{
    /// <summary>
    /// This excpetions occurs if an invalid bundle was found or provided
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class InvalidBundleException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidBundleException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidBundleException(string message) : base(message)
        {
        }
    }
}