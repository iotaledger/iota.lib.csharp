namespace Iota.Api.Core
{
    /// <summary>
    /// This class serves as base class for the different core API calls/requests
    /// </summary>
    public class IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IotaRequest"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public IotaRequest(string command)
        {
            Command = command;
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        public string Command { get; set; }
    }
}