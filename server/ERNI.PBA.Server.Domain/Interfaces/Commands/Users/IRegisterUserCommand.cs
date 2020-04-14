using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Outputs;

namespace ERNI.PBA.Server.Domain.Interfaces.Commands.Users
{
    public interface IRegisterUserCommand : ICommand<RegisterUserModel, UserModel>
    {
    }
}
