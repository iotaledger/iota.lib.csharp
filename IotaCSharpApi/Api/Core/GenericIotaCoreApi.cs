using System;
using Iota.Lib.CSharp.Api.Utils.Rest;

namespace Iota.Lib.CSharp.Api.Core
{
    public class GenericIotaCoreApi : IGenericIotaCoreApi
    {
        private string host;
        private int port;

        public string Hostname
        {
            get { return host; }
        }

        public int Port
        {
            get { return port; }
        }

        public GenericIotaCoreApi(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public TResponse Request<TRequest, TResponse>(TRequest request) where TResponse : new()
        {
            JsonWebClient jsonWebClient = new JsonWebClient();
            return jsonWebClient.GetPOSTResponseSync<TResponse>(new Uri(CreateBaseUrl()),
                new JsonSerializer().Serialize(request));
        }

        public void RequestAsync<TRequest, TResponse>(TRequest request, Action<TResponse> responseAction)
            where TResponse : new()
        {
            JsonWebClient jsonWebClient = new JsonWebClient();
            jsonWebClient.GetPOSTResponseAsync<TResponse>(new Uri(CreateBaseUrl()),
                new JsonSerializer().Serialize(request), responseAction);
        }

        private string CreateBaseUrl()
        {
            return "http://" + host + ":" + port;
        }
    }
}