using System;

namespace Iota.Lib.CSharp.Api.Core
{
    public interface IGenericIotaCoreApi
    {
        string Hostname { get; }
        int Port { get; }
        TResponse Request<TRequest, TResponse>(TRequest request) where TResponse : new();

        void RequestAsync<TRequest, TResponse>(TRequest request, Action<TResponse> responseAction)
            where TResponse : new();
    }
}