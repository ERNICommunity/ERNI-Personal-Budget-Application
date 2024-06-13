using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Graph;

namespace ERNI.PBA.Server.Business.Commands.Users;

public class SyncUserObjectIdCommand(GraphFacade graphFacade, IUserRepository userRepository, IUnitOfWork unitOfWork)
{
    public async Task Execute()
    {
        var dbUsers = await userRepository.GetAllUsers(CancellationToken.None);

        var adUsers = await graphFacade.GetUsers(CancellationToken.None);

        var userDict = dbUsers
            .Where(_ => _.ObjectId != null)
            .ToDictionary(_ => _.ObjectId!.Value);

        foreach (var u in adUsers)
        {
            if (string.IsNullOrEmpty(u.UserPrincipalName) || string.IsNullOrEmpty(u.Id))
            {
                continue;
            }

            var objectId = Guid.Parse(u.Id);

            if (!userDict.TryGetValue(objectId, out var dbUser))
            {
                var user = new User
                {
                    Username = u.UserPrincipalName,
                    ObjectId = Guid.Parse(u.Id),
                    FirstName = u.GivenName ?? "",
                    LastName = u.Surname ?? "",
                    State = UserState.New,
                    UniqueIdentifier = u.UserPrincipalName
                };

                await userRepository.AddUserAsync(user);
            }
            else if (dbUser.ObjectId != Guid.Parse(u.Id))
            {
                dbUser.ObjectId = Guid.Parse(u.Id);
            }
        }

        await unitOfWork.SaveChanges(CancellationToken.None);
    }
}
