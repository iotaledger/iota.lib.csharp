using System.Collections.Generic;

namespace Iota.Api.Standard.Model
{
    /// <summary>
    /// Thic class represents a signature
    /// </summary>
    public class Signature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Signature"/> class.
        /// </summary>
        public Signature()
        {
            SignatureFragments = new List<string>();
        }
        
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the signature fragments.
        /// </summary>
        /// <value>
        /// The signature fragments.
        /// </value>
        public List<string> SignatureFragments { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Address)}: {Address}, {nameof(SignatureFragments)}: {string.Join(",", SignatureFragments)}";
        }
    }
}