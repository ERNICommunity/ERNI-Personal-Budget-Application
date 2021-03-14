using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.Graph
{
    public class GraphFacade
    {
        private readonly GraphServiceClient _graphClient;

        public GraphFacade(GraphConfiguration config)
        {
            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(config.ClientId)
                .WithTenantId(config.TenantId)
                .WithClientSecret(config.ClientSecret)
                .Build();

            var authProvider = new ClientCredentialProvider(confidentialClientApplication);

            _graphClient = new GraphServiceClient(authProvider);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            try
            {
                var users = await _graphClient.Users.Request().Filter("country eq 'Slovakia'").GetAsync();
                return users.CurrentPage;
            }
            catch (ServiceException ex)
            {
                Console.WriteLine($"Error getting signed-in user: {ex.Message}");
                return null;
            }
        }
    }
}