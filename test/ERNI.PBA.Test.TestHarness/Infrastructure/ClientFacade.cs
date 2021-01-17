using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Models.Payloads;
using ERNI.PBA.Server.Domain.Models.Responses;
using Microsoft.AspNetCore.TestHost;

namespace ERNI.PBA.Test.TestHarness.Infrastructure
{
    public class ClientFacade : ClientFacadeBase
    {
        public ClientFacade(TestServer testServer)
            : base(testServer)
        {
        }

        #region BudgetController

        public async Task<HttpResponseMessage> GetCurrentUserBudgetByYearResponse(int year)
            => await SendGet($@"Budget/user/current/year/{year}");

        public async Task<IEnumerable<BudgetOutputModel>> GetCurrentUserBudgetByYear(int year)
            => await Deserialize<IEnumerable<BudgetOutputModel>>(await GetCurrentUserBudgetByYearResponse(year));

        #endregion

        #region UserController

        public async Task<HttpResponseMessage> RegisterUserResponse()
            => await SendPost(@"User/register");

        public async Task<UserModel> RegisterUser()
            => await Deserialize<UserModel>(await RegisterUserResponse());

        public async Task<HttpResponseMessage> CreateUserResponse(CreateUserModel createUserModel)
            => await SendPost(@"User/create", createUserModel);

        public async Task<HttpResponseMessage> GetCurrentResponse()
            => await SendGet(@"User/current");

        public async Task<UserModel> GetCurrent()
            => await Deserialize<UserModel>(await GetCurrentResponse());

        #endregion
    }
}
