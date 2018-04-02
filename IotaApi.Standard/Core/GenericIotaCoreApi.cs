using System;
using Iota.Api.Standard.Utils.Rest;

namespace Iota.Api.Standard.Core
{
    /// <summary>
    /// This class represents a generic version of the core API that is used internally 
    /// </summary>
    /// <seealso cref="IGenericIotaCoreApi" />
    public class GenericIotaCoreApi : IGenericIotaCoreApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericIotaCoreApi"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="protocol"></param>
        public GenericIotaCoreApi(string host, int port, string protocol)
        {
            Hostname = host;
            Port = port;
            Protocol = protocol;
        }

        /// <summary>
        /// Gets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        public string Hostname { get; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Protocol { get; }
        
        /// <summary>
        /// Requests the specified request.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public TResponse Request<TRequest, TResponse>(TRequest request) where TResponse : new()
        {
            JsonWebClient jsonWebClient = new JsonWebClient();
            return jsonWebClient.GetPOSTResponseSync<TResponse>(new Uri(CreateBaseUrl()),
                new JsonSerializer().Serialize(request));
        }

        /// <summary>
        /// Requests the specified request asynchronously
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="responseAction">The response action.</param>
        public void RequestAsync<TRequest, TResponse>(TRequest request, Action<TResponse> responseAction)
            where TResponse : new()
        {
            JsonWebClient jsonWebClient = new JsonWebClient();
            jsonWebClient.GetPOSTResponseAsync(new Uri(CreateBaseUrl()),
                new JsonSerializer().Serialize(request), responseAction);
        }

        private string CreateBaseUrl()
        {
            return $"{Protocol}://{Hostname}:{Port}";
        }
    }
}