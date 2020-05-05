using ERNI.PBA.Test.TestHarness.Infrastructure;
using Xunit;

namespace ERNI.PBA.Test.IntegrationTests.Infrastructure
{
    [CollectionDefinition("Database")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}
