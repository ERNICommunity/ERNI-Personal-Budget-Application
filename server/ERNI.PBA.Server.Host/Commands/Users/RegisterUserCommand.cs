using System.Security.Claims;
using ERNI.PBA.Server.Host.Model;

namespace ERNI.PBA.Server.Host.Commands.Users
{
    public class RegisterUserCommand : CommandBase<UserModel>
    {
        public ClaimsPrincipal Principal { get; set; }
    }
}
