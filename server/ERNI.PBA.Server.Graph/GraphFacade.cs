using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.Graph
{
    public class GraphFacade(GraphServiceClient graphClient)
    {
        public async Task<IEnumerable<User>> GetUsers(CancellationToken cancellationToken)
        {
            var allUsers = new List<User>();

            var usersResponse = await graphClient.Users
                .GetAsync(_ => _.QueryParameters.Filter = "country eq 'Slovakia'", cancellationToken);

            if (usersResponse is null)
            {
                return allUsers;
            }

            var pageIterator = PageIterator<User, UserCollectionResponse>.CreatePageIterator(
            graphClient,
            usersResponse,
            user =>
            {
                allUsers.Add(user);
                return true;
            });

            await pageIterator.IterateAsync(cancellationToken);

            return allUsers;
        }
    }
}