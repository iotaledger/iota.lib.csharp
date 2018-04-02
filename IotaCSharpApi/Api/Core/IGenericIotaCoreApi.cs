using System;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// This interface abstracts a generic version of the core api that is used internally.
    /// </summary>
    public interface IGenericIotaCoreApi
    {
        /// <summary>
        /// Gets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        string Hostname { get; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        int Port { get; }

        /// <summary>
        /// 
        /// </summary>
        string Protocol { get; }

        /// <summary>
        /// Requests the specified request.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        TResponse Request<TRequest, TResponse>(TRequest request) where TResponse : new();

        /// <summary>
        /// Requests the specified request asynchronously
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="responseAction">The response action.</param>
        void RequestAsync<TRequest, TResponse>(TRequest request, Action<TResponse> responseAction)
            where TResponse : new();
    }
}