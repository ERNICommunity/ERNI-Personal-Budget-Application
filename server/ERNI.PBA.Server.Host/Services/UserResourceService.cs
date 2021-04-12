using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Services;
using ERNI.PBA.Server.Domain.Models.Payloads;
using Newtonsoft.Json;

namespace ERNI.PBA.Server.Host.Services
{
    public sealed class UserResourceService : IUserResourceService, IDisposable
    {
        private readonly Uri _requestUri;
        private static readonly HttpClient _client = new();

        public UserResourceService(Uri requestUri)
        {
            _requestUri = requestUri;

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IEnumerable<UserResourceModel>> GetAsync()
        {
            var response = await _client.GetAsync(_requestUri);

            var responseBody = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<UserResourceModel[]>(responseBody);
        }

        public void Dispose() => _client?.Dispose();
    }
}