namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// This class represents the response of <see cref="GetNodeInfoRequest"/>
    /// </summary>
    /// <seealso cref="Iota.Lib.CSharp.Api.Core.IotaResponse" />
    public class GetNodeInfoResponse : IotaResponse
    {
        /// <summary>
        /// Name of the IOTA software you're currently using (IRI stands for Initial Reference Implementation).
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// The version of the IOTA software you're currently running.
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// Available cores on your machine for JRE.  
        /// </summary>
        public int JreAvailableProcessors { get; set; }

        /// <summary>
        /// The amount of free memory in the Java Virtual Machine.
        /// </summary>
        public long JreFreeMemory { get; set; }

        /// <summary>
        /// The maximum amount of memory that the Java virtual machine will attempt to use.
        /// </summary>
        public long JreMaxMemory { get; set; }

        /// <summary>
        /// The total amount of memory in the Java virtual machine.
        /// </summary>
        public long JreTotalMemory { get; set; }

        /// <summary>
        /// Latest milestone that was signed off by the coordinator. 
        /// </summary>
        public string LatestMilestone { get; set; }

        /// <summary>
        /// Index of the latest milestone.
        /// </summary>
        public long LatestMilestoneIndex { get; set; }

        /// <summary>
        /// The latest milestone which is solid and is used for sending transactions. 
        /// For a milestone to become solid your local node must basically approve the subtangle of coordinator-approved transactions,
        /// and have a consistent view of all referenced transactions. 
        /// </summary>
        public string LatestSolidSubtangleMilestone { get; set; }

        /// <summary>
        ///  Index of the latest solid subtangle.
        /// </summary>
        public long LatestSolidSubtangleMilestoneIndex { get; set; }

        /// <summary>
        /// Number of neighbors you are directly connected with.
        /// </summary>
        public long Neighbors { get; set; }

        /// <summary>
        /// Packets which are currently queued up
        /// </summary>
        public long PacketsQueueSize { get; set; }

        /// <summary>
        /// Current UNIX timestamp.
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// Number of tips in the network.
        /// </summary>
        public long Tips { get; set; }

        /// <summary>
        /// Transactions to request during syncing process.
        /// </summary>
        public long TransactionsToRequest { get; set; }

        public string JreVersion { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return
                $"{nameof(AppName)}: {AppName}, {nameof(AppVersion)}: {AppVersion}, {nameof(JreAvailableProcessors)}: {JreAvailableProcessors}, {nameof(JreFreeMemory)}: {JreFreeMemory}, {nameof(JreMaxMemory)}: {JreMaxMemory}, {nameof(JreTotalMemory)}: {JreTotalMemory}, {nameof(LatestMilestone)}: {LatestMilestone}, {nameof(LatestMilestoneIndex)}: {LatestMilestoneIndex}, {nameof(LatestSolidSubtangleMilestone)}: {LatestSolidSubtangleMilestone}, {nameof(LatestSolidSubtangleMilestoneIndex)}: {LatestSolidSubtangleMilestoneIndex}, {nameof(Neighbors)}: {Neighbors}, {nameof(PacketsQueueSize)}: {PacketsQueueSize}, {nameof(Time)}: {Time}, {nameof(Tips)}: {Tips}, {nameof(TransactionsToRequest)}: {TransactionsToRequest}";
        }
    }
}