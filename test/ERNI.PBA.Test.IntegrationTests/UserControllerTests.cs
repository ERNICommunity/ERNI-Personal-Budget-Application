using System.Net;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Models.Payloads;
using ERNI.PBA.Test.TestHarness.Infrastructure;
using Xunit;
using Xunit.Abstractions;
using static ERNI.PBA.Test.IntegrationTests.Infrastructure.Traits.Values;
using static ERNI.PBA.Test.IntegrationTests.Infrastructure.Traits.Names;

namespace ERNI.PBA.Test.IntegrationTests
{
    [Collection("Database")]
    public class UserControllerTests : TestBase
    {
        public UserControllerTests(DatabaseFixture databaseFixture, ITestOutputHelper output)
            : base(databaseFixture, output)
        {
            PrepareDatabase();
        }

        [Fact]
        [Trait(Category, CI)]
        public async Task CreateUser_Then_RegisterUser()
        {
            // Arrange
            var createUserModel = new CreateUserModel
            {
                FirstName = "Mister",
                LastName = "Geppetto",
                Email = "gemi@erni.sk",
                Amount = 350,
                Year = 2020,
                State = UserState.Active
            };

            var user = new AzureIdentity
            {
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName,
                UserName = createUserModel.Email,
                UniqueIndetifier = "wZsKLrTLeFejWrXTSxqMcJnbqNmUjsRnCIGyMWEJsmA"
            };

            SetIdentity(AzureIdentity.Admin);

            // Act
            var createUserResponse = await Client.CreateUserResponse(createUserModel);
            SetIdentity(user);
            var registerUserResponse = await Client.RegisterUserResponse();

            // Assert
            Assert.Equal(HttpStatusCode.OK, createUserResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, registerUserResponse.StatusCode);
        }
    }
}
