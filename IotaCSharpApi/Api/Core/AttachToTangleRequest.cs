namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Created by Adrian on 28.04.2016.
    /// </summary>
    public class AttachToTangleRequest : IotaRequest
    {
        private const int MinWeightMagnitudeMin = 18;

        private int _minWeightMagnitude = MinWeightMagnitudeMin;

        public AttachToTangleRequest(string trunkTransaction, string branchTransaction, string[] trytes,
            int minWeightMagnitude = 18) : base(Core.Command.AttachToTangle.GetCommandString())
        {
            TrunkTransaction = trunkTransaction;
            BranchTransaction = branchTransaction;
            Trytes = trytes;
            MinWeightMagnitude = minWeightMagnitude;

            if (Trytes == null)
                Trytes = new string[0];
        }

        /// <summary>
        /// Proof of Work intensity. Minimum value is 18
        /// </summary>
        public int MinWeightMagnitude
        {
            get { return _minWeightMagnitude; }
            set
            {
                if (value > MinWeightMagnitudeMin)
                    _minWeightMagnitude = value;
            }
        }

        /// <summary>
        /// Trunk transaction to approve.
        /// </summary>
        public string TrunkTransaction { get; set; }

        /// <summary>
        /// Branch transaction to approve.
        /// </summary>
        public string BranchTransaction { get; set; }

        /// <summary>
        /// List of trytes (raw transaction data) to attach to the tangle.
        /// </summary>
        public string[] Trytes { get; set; }
    }
}