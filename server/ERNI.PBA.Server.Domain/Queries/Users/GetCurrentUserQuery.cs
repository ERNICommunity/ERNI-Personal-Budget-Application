using System.Security.Claims;
using ERNI.PBA.Server.Domain.Output;

namespace ERNI.PBA.Server.Domain.Queries.Users
{
    public class GetCurrentUserQuery : QueryBase<UserModel>
    {
        public ClaimsPrincipal Principal { get; set; }
    }
}
