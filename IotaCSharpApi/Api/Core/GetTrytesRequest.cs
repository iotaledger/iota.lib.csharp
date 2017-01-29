namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Created by Adrian on 28.04.2016.
    /// </summary>
    public class GetTrytesRequest : IotaRequest
    {
        public GetTrytesRequest() : base(Core.Command.GetTrytes.GetCommandString())
        {
        }

        public string[] Hashes { get; set; }
    }
}