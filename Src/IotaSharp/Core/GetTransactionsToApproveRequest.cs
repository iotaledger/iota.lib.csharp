namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetTransactionsToApproveRequest:IotaRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="reference"></param>
        public GetTransactionsToApproveRequest(int depth, string reference)
            : base(Core.Command.GetTransactionsToApprove)
        {
            Depth = depth;
            Reference = reference;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Reference { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Depth)}: {Depth},{nameof(Reference)}: {Reference}";
        }
    }
}
