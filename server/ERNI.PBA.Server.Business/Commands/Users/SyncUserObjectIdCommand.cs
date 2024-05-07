using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Graph;

namespace ERNI.PBA.Server.Business.Commands.Users;

public class SyncUserObjectIdCommand(GraphFacade graphFacade, IUserRepository userRepository, IUnitOfWork unitOfWork)
{
    public async Task Execute()
    {
        var dbUsers = await userRepository.GetAllUsers(CancellationToken.None);

        var adUsers = await graphFacade.GetUsers(CancellationToken.None);

        var userDict = dbUsers.ToDictionary(_ => _.Username.ToUpperInvariant());

        foreach (var u in adUsers)
        {
            if (string.IsNullOrEmpty(u.UserPrincipalName) || string.IsNullOrEmpty(u.Id))
            {
                continue;
            }

            if (userDict.TryGetValue(u.UserPrincipalName.ToUpperInvariant(), out var dbUser) && dbUser.ObjectId != Guid.Parse(u.Id))
            {
                dbUser.ObjectId = Guid.Parse(u.Id);
            }
        }

        await unitOfWork.SaveChanges(CancellationToken.None);
    }
}
