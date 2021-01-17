using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ERNI.PBA.Server.Host.Extensions
{
    public static class HostingEnvironmentExtensions
    {
        public static bool IsInt(this IWebHostEnvironment hostingEnvironment)
        {
            return hostingEnvironment.IsEnvironment("IntegrationTesting");
        }
    }
}
