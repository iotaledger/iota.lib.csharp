﻿namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// This class represents the response of <see cref="RemoveNeighborsRequest"/>
    /// </summary>
    public class RemoveNeighborsResponse : IotaResponseBase
    {
        /// <summary>
        /// Gets or sets the number of removed neighbors.
        /// </summary>
        /// <value>
        /// The removed neighbors.
        /// </value>
        public long RemovedNeighbors { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(RemovedNeighbors)}: {RemovedNeighbors}";
        }
    }
}