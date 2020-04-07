using System.Security.Claims;
using ERNI.PBA.Server.Host.Model;

namespace ERNI.PBA.Server.Host.Queries.Users
{
    public class GetCurrentUserQuery : QueryBase<UserModel>
    {
        public ClaimsPrincipal Principal { get; set; }
    }
}
