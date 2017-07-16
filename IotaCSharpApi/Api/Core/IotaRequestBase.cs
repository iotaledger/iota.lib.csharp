namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// This class serves as base class for the different core API calls/requests
    /// </summary>
    public abstract class IotaRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IotaRequestBase"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public IotaRequestBase(string command)
        {
            this.Command = command;
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