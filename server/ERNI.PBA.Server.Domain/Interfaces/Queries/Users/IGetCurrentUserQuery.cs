using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Models.Outputs;

namespace ERNI.PBA.Server.Domain.Interfaces.Queries.Users
{
    public interface IGetCurrentUserQuery : IQuery<UserModel>
    {
    }
}
