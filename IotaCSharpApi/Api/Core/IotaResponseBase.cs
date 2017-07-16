namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// This class represents the base class of different core API response classes
    /// </summary>
    public abstract class IotaResponseBase
    {
        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public long Duration { get; set; }
    }
}