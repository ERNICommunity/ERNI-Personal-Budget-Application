using ERNI.PBA.Server.Domain.Models.Responses;
using ERNI.PBA.Server.Domain.Models.Responses.PendingRequests;
using Swashbuckle.AspNetCore.Filters;

namespace ERNI.PBA.Server.Host.Examples
{
    public class RequestExample : IExamplesProvider<RequestModel[]>
    {
        public RequestModel[] GetExamples()
        {
            return new[]
            {
                new RequestModel
                {
                    Id = 1,
                    Title = "Deadpool",
                    Amount = 7.5M,
                    Year = 2018,
                    User = new UserOutputModel
                    {
                        Id = 1,
                        FirstName = "Johnny",
                        LastName = "Cash"
                    },
                },
                new RequestModel
                {
                    Id = 2,
                    Title = "Massage",
                    Amount = 10M,
                    Year = 2018,
                    User = new UserOutputModel
                    {
                        Id = 1,
                        FirstName = "Hans",
                        LastName = "Gruber"
                    },
                },
            };
        }
    }
}
