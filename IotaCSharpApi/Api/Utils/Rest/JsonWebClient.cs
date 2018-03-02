using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using Iota.Lib.CSharp.Api.Core;
using Iota.Lib.CSharp.Api.Exception;
using Newtonsoft.Json;

namespace Iota.Lib.CSharp.Api.Utils.Rest
{
    internal static class JsonWebClient
    {
        public static TResponse GetPOSTResponseSync<TResponse>(Uri uri, string data)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);

            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Method = "POST";
            request.AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate);
            request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.Headers.Add("X-IOTA-API-Version", "1");
            request.Headers.Add("Origin", "iota.lib.csharp");
            request.Headers.Add("Accept-Language", "de-DE,de;q=0.8,en-US;q=0.6,en;q=0.4 ");
            request.KeepAlive = false;
            request.Timeout = 300000;
            request.ReadWriteTimeout = 300000;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(data);

            request.ContentLength = bytes.Length;

            Stream requestStream = request.GetRequestStream();
            {
                // Send the data.
                requestStream.Write(bytes, 0, bytes.Length);
            }
            
            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseString = reader.ReadToEnd();

                    return JsonConvert.DeserializeObject<TResponse>(responseString);
                }
            }
        }

        public static async Task<TResponse> GetPOSTResponseAsync<TResponse>(Uri uri, string data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = "POST";
            request.ContentType = "application-type/json;charset=utf-8";

            byte[] payload = Encoding.UTF8.GetBytes(data);

            request.ContentLength = payload.Length;

            using (var requestStream = request.GetRequestStream())
            {
                await requestStream.WriteAsync(payload, 0, payload.Length);
            }

            var rsp = (HttpWebResponse)await request.GetResponseAsync();
            using (var stream = new StreamReader(rsp.GetResponseStream(), Encoding.UTF8))
            {
                return JsonConvert.DeserializeObject<TResponse>(await stream.ReadToEndAsync());
            }
        }
    }
}