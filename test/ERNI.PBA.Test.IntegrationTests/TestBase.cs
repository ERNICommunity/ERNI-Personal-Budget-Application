using System.Diagnostics;
using ERNI.PBA.Test.TestHarness.Infrastructure;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace ERNI.PBA.Test.IntegrationTests
{
    public abstract class TestBase
    {
        protected TestBase(DatabaseFixture databaseFixture, ITestOutputHelper output)
        {
            Output = output;
            Host = databaseFixture.GetHost();
            Database = new DatabaseFacade(Host.Services, databaseFixture.DatabaseName);
            databaseFixture.DisposeAction = FixtureDispose;
            Client = databaseFixture.StartOrGetClient();
        }

        protected ClientFacade Client { get; }
        protected ITestOutputHelper Output { get; }
        protected IHost Host { get; }
        protected DatabaseFacade Database { get; }

        private void FixtureDispose()
        {
            Debug.WriteLine("Disposing test fixture");

            Database.Delete();
        }

        protected void PrepareDatabase()
        {
            Database.Delete();
            Database.Initialize();
        }
    }
}
