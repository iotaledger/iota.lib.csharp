using System.Collections.Generic;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class FindTransactionsRequest : IotaRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundles"></param>
        /// <param name="addresses"></param>
        /// <param name="tags"></param>
        /// <param name="approvees"></param>
        public FindTransactionsRequest(
             List<string> addresses, List<string> tags, List<string> approvees, List<string> bundles)
            : base(Core.Command.FindTransactions)
        {
            Addresses = addresses;
            Tags = tags;
            Approvees = approvees;
            Bundles = bundles;
        }
        
        /// <summary>
        /// Gets or sets the addresses.
        /// </summary>
        /// <value>
        /// The addresses.
        /// </value>
        public List<string> Addresses { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public List<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the approvees.
        /// </summary>
        /// <value>
        /// The approvees.
        /// </value>
        public List<string> Approvees { get; set; }

        /// <summary>
        /// Gets or sets the bundles.
        /// </summary>
        /// <value>
        /// The bundles.
        /// </value>
        public List<string> Bundles { get; set; }

    }
}
