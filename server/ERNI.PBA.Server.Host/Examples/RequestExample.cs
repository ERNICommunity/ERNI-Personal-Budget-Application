using ERNI.PBA.Server.Host.Model.PendingRequests;
using Swashbuckle.AspNetCore.Examples;

namespace ERNI.PBA.Server.Host.Examples
{
    public class RequestExample : IExamplesProvider
    {
        public object GetExamples()
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
                new Model.PendingRequests.RequestModel
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