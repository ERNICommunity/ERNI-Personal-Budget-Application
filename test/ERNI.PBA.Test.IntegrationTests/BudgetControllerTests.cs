using System.Threading.Tasks;
using ERNI.PBA.Test.TestHarness.Infrastructure;
using Xunit;
using Xunit.Abstractions;
using static ERNI.PBA.Test.IntegrationTests.Infrastructure.Traits.Values;
using static ERNI.PBA.Test.IntegrationTests.Infrastructure.Traits.Names;

namespace ERNI.PBA.Test.IntegrationTests
{
    [Collection("Database")]
    public class BudgetControllerTests : TestBase
    {
        public BudgetControllerTests(DatabaseFixture databaseFixture, ITestOutputHelper output)
            : base(databaseFixture, output)
        {
            PrepareDatabase();
        }

        [Fact]
        [Trait(Category, CI)]
        public async Task GetEmployeeCode()
        {
            // Act
            var budgetOutputModels = await Client.GetCurrentUserBudgetByYear(2020);

            // Assert
            Assert.Empty(budgetOutputModels);
        }
    }
}
