using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using Iota.Lib.CSharp.Api.Core;
using Iota.Lib.CSharp.Api.Exception;
using Newtonsoft.Json;

namespace Iota.Lib.CSharp.Api.Utils.Rest
{
    internal class JsonWebClient
    {
        public TResponse GetPOSTResponseSync<TResponse>(Uri uri, string data)
        {
            Console.WriteLine("request: " + data);

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

            try
            {
                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseString = reader.ReadToEnd();

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Console.WriteLine("response: " + responseString);
                            return JsonConvert.DeserializeObject<TResponse>(responseString);
                        }

                        throw new IotaApiException(JsonConvert.DeserializeObject<ErrorResponse>(responseString).Error);
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine("catched: " + ex.ToString() + ex.Message);

                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    String errorResponse = reader.ReadToEnd();
                    throw new IotaApiException(JsonConvert.DeserializeObject<ErrorResponse>(errorResponse).Error);
                }
            }
        }

        public void GetPOSTResponseAsync<TResponse>(Uri uri, string data, Action<TResponse> callback)
        {
            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(uri);

            request.Method = "POST";
            request.ContentType = "application-type/json;charset=utf-8";

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] bytes = encoding.GetBytes(data);

            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                // Send the data.
                requestStream.Write(bytes, 0, bytes.Length);
            }

            request.BeginGetResponse((x) =>
            {
                using (HttpWebResponse response = (HttpWebResponse) request.EndGetResponse(x))
                {
                    if (callback != null)
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                            string responseString = reader.ReadToEnd();
                            callback(JsonConvert.DeserializeObject<TResponse>(responseString));
                            ;
                        }
                    }
                }
            }, request);
        }
    }
}