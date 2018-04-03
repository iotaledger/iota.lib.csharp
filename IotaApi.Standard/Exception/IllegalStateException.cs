namespace Iota.Api.Exception
{
    /// <summary>
    ///     This exception occurs when an illegal state is encountered
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class IllegalStateException : System.Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="IllegalStateException" /> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public IllegalStateException(string error) : base(error)
        {
        }
    }
}