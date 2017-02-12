using System.Collections.Generic;
using System.Linq;

namespace Iota.Lib.CSharp.Api.Model
{
    /// <summary>
    /// This class represents the Inputs
    /// </summary>
    public class Inputs
    {
        /// <summary>
        /// Gets or sets the inputs list.
        /// </summary>
        /// <value>
        /// The inputs list.
        /// </value>
        public List<Input> InputsList { get; set; }

        /// <summary>
        /// Gets or sets the total balance.
        /// </summary>
        /// <value>
        /// The total balance.
        /// </value>
        public long TotalBalance { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return
                $"Inputs:\n {string.Join(",", InputsList.Select(i => "[" + i + "]" + "\n"))}{nameof(TotalBalance)}: {TotalBalance}";
        }
    }
}