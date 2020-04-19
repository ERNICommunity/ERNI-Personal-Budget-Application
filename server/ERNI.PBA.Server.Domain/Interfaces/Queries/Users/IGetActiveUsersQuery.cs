using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Models.Responses;

namespace ERNI.PBA.Server.Domain.Interfaces.Queries.Users
{
    public interface IGetActiveUsersQuery : IQuery<IEnumerable<UserModel>>
    {
    }
}
