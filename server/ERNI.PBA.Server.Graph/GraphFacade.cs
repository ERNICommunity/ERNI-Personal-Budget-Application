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
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(config.ClientId)
                .WithTenantId(config.TenantId)
                .WithClientSecret(config.ClientSecret)
                .Build();

            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);

            _graphClient = new GraphServiceClient(authProvider);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var allUsers = new List<User>();

            var users = await _graphClient.Users.Request().Filter("country eq 'Slovakia'").GetAsync();

            while (users.Count > 0)
            {
                allUsers.AddRange(users);
                if (users.NextPageRequest != null)
                {
                    users = await users.NextPageRequest.GetAsync();
                }
                else
                {
                    break;
                }
            }

            return allUsers;
        }
    }
}