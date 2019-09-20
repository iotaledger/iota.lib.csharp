using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using IotaSharp.Core;
using IotaSharp.Exception;
using Newtonsoft.Json;

namespace IotaSharp.Utils
{
    internal class JsonWebClient
    {
        public TResponse GetPOSTResponseSync<TResponse>(Uri uri, string data)
        {
            var request = WebRequest.CreateHttp(uri);
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Method = "POST";
            request.AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate);
            request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.Headers.Add("X-IOTA-API-Version", "1");
            request.Headers.Add("Origin", "IotaSharp");
            request.KeepAlive = false;
            request.Timeout = 300000;
            request.ReadWriteTimeout = 300000;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(data);
            request.ContentLength = bytes.Length;

            using (var requestStream = request.GetRequestStream())
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
                        Debug.Assert(stream != null, nameof(stream) + " != null");

                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseString = reader.ReadToEnd();

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return JsonConvert.DeserializeObject<TResponse>(responseString);
                        }

                        throw new IotaApiException(JsonConvert.DeserializeObject<ErrorResponse>(responseString).Error);
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    {
                        Debug.Assert(stream != null, nameof(stream) + " != null");

                        using (var reader = new StreamReader(stream))
                        {
                            string errorResponse = reader.ReadToEnd();

                            HttpWebResponse response = (HttpWebResponse) ex.Response;

                            if (response.StatusCode == HttpStatusCode.BadRequest)
                                throw new ArgumentException(errorResponse);
                            if (response.StatusCode == HttpStatusCode.Unauthorized)
                                throw new IllegalAccessException("401" + errorResponse);
                            if (response.StatusCode == HttpStatusCode.InternalServerError)
                                throw new IllegalAccessException("500" + errorResponse);


                            throw new IotaApiException(
                                JsonConvert.DeserializeObject<ErrorResponse>(errorResponse).Error);
                        }
                    }
                }

                throw;
            }
        }
    }
}
