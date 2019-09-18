namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class AttachToTangleRequest : IotaRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trunkTransaction"></param>
        /// <param name="branchTransaction"></param>
        /// <param name="minWeightMagnitude"></param>
        /// <param name="trytes"></param>
        public AttachToTangleRequest(
            string trunkTransaction, string branchTransaction,
            int minWeightMagnitude, string[] trytes)
            : base(Core.Command.AttachToTangle)
        {
            TrunkTransaction = trunkTransaction;
            BranchTransaction = branchTransaction;
            MinWeightMagnitude = minWeightMagnitude;
            Trytes = trytes;
        }

        /// <summary>
        /// 
        /// </summary>
        public string TrunkTransaction { get; }

        /// <summary>
        /// 
        /// </summary>
        public string BranchTransaction { get; }

        /// <summary>
        /// 
        /// </summary>
        public int MinWeightMagnitude { get; }

        /// <summary>
        /// 
        /// </summary>
        public string[] Trytes { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                $"{nameof(MinWeightMagnitude)}: {MinWeightMagnitude}, {nameof(TrunkTransaction)}: {TrunkTransaction}, {nameof(BranchTransaction)}: {BranchTransaction}, {nameof(Trytes)}: {Trytes}";
        }
    }
}
