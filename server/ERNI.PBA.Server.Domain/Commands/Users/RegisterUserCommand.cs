using System.Security.Claims;
using ERNI.PBA.Server.Domain.Output;

namespace ERNI.PBA.Server.Domain.Commands.Users
{
    public class RegisterUserCommand : CommandBase<UserModel>
    {
        public ClaimsPrincipal Principal { get; set; }
    }
}
