using System;
using System.Net.Http;
using System.Threading.Tasks;
using ERNI.PBA.Test.TestHarness.Extensions;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;

namespace ERNI.PBA.Test.TestHarness.Infrastructure
{
    public abstract class ClientFacadeBase
    {
        private readonly Func<HttpClient> _clientFactory;

        public static readonly Func<TestServer, HttpClient> LocalServer = x =>
        {
            var result = x.CreateClient();
            result.BaseAddress = new Uri("http://localhost/api/");
            return result;
        };

        protected ClientFacadeBase(TestServer testServer)
            : this(() => LocalServer(testServer))
        {
        }

        protected ClientFacadeBase(string baseAddress)
            : this(() => new HttpClient() { BaseAddress = new Uri(baseAddress) })
        {
        }

        protected ClientFacadeBase(Func<HttpClient> clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<T> Deserialize<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public async Task<HttpResponseMessage> SendGet(string requestUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            return await Send(request);
        }

        public async Task<HttpResponseMessage> SendPost(string requestUri, object data = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
                .AddJsonContent(data);

            return await Send(request);
        }

        public async Task<HttpResponseMessage> SendPut(string requestUri, object data = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
                .AddJsonContent(data);

            return await Send(request);
        }

        public async Task<HttpResponseMessage> SendDelete(string requestUri, object data = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri)
                .AddJsonContent(data);

            return await Send(request);
        }

        public async Task<HttpResponseMessage> Send(HttpRequestMessage request)
        {
            using (var client = CreateClient())
            {
                return await client.SendAsync(request);
            }
        }

        public HttpClient CreateClient()
        {
            var result = _clientFactory();
            return result;
        }
    }
}
