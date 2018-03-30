namespace Iota.Api.Standard.Exception
{
    /// <summary>
    /// This exception encapsulates an error that occured while communicating with the node (for example during a core API call)
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class IotaApiException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IotaApiException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public IotaApiException(string error) : base(error)
        {
        }
    }
}