using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Response of <see cref="AttachToTangleRequest"/>
    /// </summary>
    public class AttachToTangleResponse
    {
        /// <summary>
        /// Gets or sets the trytes.
        /// </summary>
        /// <value>
        /// The trytes.
        /// </value>
        public List<string> Trytes { get; set; }

        public override string ToString()
        {
            return $"{nameof(Trytes)}: {string.Join(",", Trytes)}";
        }
    }
}