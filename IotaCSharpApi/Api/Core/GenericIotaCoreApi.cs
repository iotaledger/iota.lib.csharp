using System;
using System.Threading.Tasks;
using Iota.Lib.CSharp.Api.Utils.Rest;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// This class represents a generic version of the core API that is used internally 
    /// </summary>
    /// <seealso cref="Iota.Lib.CSharp.Api.Core.IGenericIotaCoreApi" />
    public class GenericIotaCoreApi : IGenericIotaCoreApi
    {
        private string host;
        private int port;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericIotaCoreApi"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public GenericIotaCoreApi(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        /// <summary>
        /// Gets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        public string Hostname
        {
            get { return host; }
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port
        {
            get { return port; }
        }

        /// <summary>
        /// Requests the specified request.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public TResponse Request<TRequest, TResponse>(TRequest request) where TResponse : new()
        {
            return JsonWebClient.GetPOSTResponseSync<TResponse>(new Uri(CreateBaseUrl()), new JsonSerializer().Serialize(request));
        }

        /// <summary>
        /// Requests the specified request asynchronously
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request.</param>
        public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request) where TResponse : new()
        {
            return await JsonWebClient.GetPOSTResponseAsync<TResponse>(new Uri(CreateBaseUrl()), new JsonSerializer().Serialize(request));
        }

        private string CreateBaseUrl()
        {
            return "http://" + host + ":" + port;
        }
    }
}