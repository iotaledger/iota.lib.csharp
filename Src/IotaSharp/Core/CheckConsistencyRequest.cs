namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class CheckConsistencyRequest : IotaRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tails"></param>
        public CheckConsistencyRequest(string[] tails) : base(Core.Command.CheckConsistency)
        {
            Tails = tails;
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] Tails { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Tails)}: {string.Join(",", Tails)}";
        }
    }
}
