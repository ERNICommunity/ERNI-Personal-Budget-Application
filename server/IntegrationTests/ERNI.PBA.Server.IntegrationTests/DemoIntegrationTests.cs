using System;
using System.Net;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Host;
using ERNI.PBA.Server.IntegrationTests.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERNI.PBA.Server.IntegrationTests
{
    [TestClass]
    public sealed class DemoIntegrationTests : IDisposable
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public DemoIntegrationTests() => _factory = new CustomWebApplicationFactory<Startup>();

        [TestMethod]
        public async Task GetBudgetTypes()
        {
            using var client = _factory.WithWebHostBuilder(b =>
            {
                b.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "Test", options => { });
                });
            }).CreateClient(new WebApplicationFactoryClientOptions());

            var response = await client.GetAsync(new Uri("/api/budget/types"));

            var result = await response.Deserialize<BudgetType[]>();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }



        [TestMethod]
        public async Task ValidRequestCreatesBudget()
        {
            using var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var response = await client.GetAsync(new Uri("/api/version"));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        public void Dispose() => _factory.Dispose();
    }
}