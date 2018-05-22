using Swashbuckle.AspNetCore.Examples;

namespace ERNI.PBA.Server.Host.Examples
{
    public class PendingRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new[]
            {
                new Model.PendingRequests.RequestModel
                {
                    Id = 1,
                    Title = "Deadpool",
                    Amount = 7.5M,
                    Year = 2018,
                    User = new Model.PendingRequests.UserModel
                    {
                        Id = 1,
                        FirtName = "Johnny",
                        LastName = "Cash"
                    }
                },
                new Model.PendingRequests.RequestModel
                {
                    Id = 2,
                    Title = "Massage",
                    Amount = 10M,
                    Year = 2018,
                    User = new Model.PendingRequests.UserModel
                    {
                        Id = 1,
                        FirtName = "Hans",
                        LastName = "Gruber"
                    }
                },
            };
        }
    }
}