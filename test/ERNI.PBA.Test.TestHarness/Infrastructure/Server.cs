using System;
using Autofac.Extensions.DependencyInjection;
using ERNI.PBA.Server.Host;
using ERNI.PBA.Test.TestHarness.Authentication;
using ERNI.PBA.Test.TestHarness.Extensions;
using ERNI.PBA.Test.TestHarness.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ERNI.PBA.Test.TestHarness.Infrastructure
{
    public static class Server
    {
        public static IHost StartBackendServer(
            string environment = "IntegrationTesting",
            string databasename = "PbaIntTest")
        {
            var connectionString = $@"Server=.\SQLEXPRESS;Database={databasename};Trusted_Connection=True;";
            Environment.SetEnvironmentVariable("ConnectionStrings:ConnectionString", connectionString);

            var webHostBuilder = Host.CreateDefaultBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseStartup<Startup>()
                        .ConfigureStubs()
                        .UseEnvironment(environment)
                        .UseTestServer();
                });

            var host = webHostBuilder.Start();

            return host;
        }

        private static IWebHostBuilder ConfigureStubs(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<IIdentityService, IdentityService>();
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = Constants.TestAuthenticateScheme;
                    options.DefaultChallengeScheme = Constants.TestAuthenticateScheme;
                }).AddTestAuth(options => { });
            });

            return webHostBuilder;
        }
    }
}
