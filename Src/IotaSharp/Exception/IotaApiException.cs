namespace IotaSharp.Exception
{
    /// <summary>
    /// 
    /// </summary>
    public class IotaApiException : System.Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        public IotaApiException(string error) : base(error)
        {
        }
    }
}
