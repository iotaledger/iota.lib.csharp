namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// This class represents the core API request 'AttachToTangle'.
    /// It is used to attach trytes to the tangle.
    /// </summary>
    public class AttachToTangleRequest : IotaRequestBase
    {
        private const int MinWeightMagnitudeMin = 18;
        private int _minWeightMagnitude = MinWeightMagnitudeMin;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachToTangleRequest"/> class.
        /// </summary>
        /// <param name="trunkTransaction">The trunk transaction.</param>
        /// <param name="branchTransaction">The branch transaction.</param>
        /// <param name="trytes">The trytes.</param>
        /// <param name="minWeightMagnitude">The minimum weight magnitude.</param>
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
            get => _minWeightMagnitude;
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(MinWeightMagnitude)}: {MinWeightMagnitude}, {nameof(TrunkTransaction)}: {TrunkTransaction}, {nameof(BranchTransaction)}: {BranchTransaction}, {nameof(Trytes)}: {Trytes}";
        }
    }
}