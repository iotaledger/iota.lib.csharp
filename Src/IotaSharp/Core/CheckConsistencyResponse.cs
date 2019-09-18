namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class CheckConsistencyResponse : IotaResponse
    {
        /// <summary>
        /// Gets the state
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// If state is false, this provides information on the cause of the inconsistency.
        /// </summary>
        public string Info { get; set; }
    }
}
