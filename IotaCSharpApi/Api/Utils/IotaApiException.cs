namespace Iota.Lib.CSharp.Api.Utils
{
    public class IotaApiException : System.Exception
    {
        public IotaApiException(string error) : base(error)
        {
        }
    }
}