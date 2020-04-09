using System.Security.Claims;
using ERNI.PBA.Server.Domain.Models.Outputs;

namespace ERNI.PBA.Server.Domain.Queries.Users
{
    public class GetCurrentUserQuery : QueryBase<UserModel>
    {
        public ClaimsPrincipal Principal { get; set; }
    }
}
