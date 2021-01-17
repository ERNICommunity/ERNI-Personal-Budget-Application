using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace ERNI.PBA.Test.TestHarness.Extensions
{
    public static class TestingExtensions
    {
        public static HttpRequestMessage AddJsonContent(this HttpRequestMessage request, object obj)
        {
            if (obj != null)
            {
                var content = JsonConvert.SerializeObject(obj);
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            return request;
        }
    }
}
