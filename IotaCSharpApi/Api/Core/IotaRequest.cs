namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Created by Adrian on 28.04.2016.
    /// </summary>
    public class IotaRequest
    {
        public IotaRequest(string command)
        {
            this.Command = command;
        }

        //[JsonProperty(PropertyName = "command")]
        public string Command { get; set; }
    }
}