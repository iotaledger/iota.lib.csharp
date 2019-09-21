namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class RemoveNeighborsResponse : IotaResponse
    {
        /// <summary>
        /// Gets or sets the number of removed neighbors.
        /// </summary>
        /// <value>
        /// The removed neighbors.
        /// </value>
        public long RemovedNeighbors { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(RemovedNeighbors)}: {RemovedNeighbors}";
        }
    }
}
