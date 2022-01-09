using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ERNI.PBA.Server.IntegrationTests.Utils
{
    public static class HttpResponseExtensions
    {
        public static async Task<T> Deserialize<T>(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}