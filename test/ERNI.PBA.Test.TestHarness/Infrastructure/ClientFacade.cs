using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
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

        public async Task<HttpResponseMessage> GetCurrentUserBudgetByYearResponse(int year)
            => await SendGet($@"Budget/user/current/year/{year}");

        public async Task<IEnumerable<BudgetOutputModel>> GetCurrentUserBudgetByYear(int year)
            => await Deserialize<IEnumerable<BudgetOutputModel>>(await GetCurrentUserBudgetByYearResponse(year));
    }
}
