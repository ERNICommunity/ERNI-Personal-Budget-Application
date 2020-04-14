using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Models.Payloads;

namespace ERNI.PBA.Server.Domain.Interfaces.Commands.Users
{
    public interface IUpdateUserCommand : ICommand<UpdateUserModel>
    {
    }
}
