using System.Collections.Generic;
using System.Security.Claims;
using ERNI.PBA.Server.Domain.Models.Outputs;

namespace ERNI.PBA.Server.Domain.Queries.Users
{
    public class GetSubordinateUsersQuery : QueryBase<IEnumerable<UserModel>>
    {
        public ClaimsPrincipal Principal { get; set; }
    }
}
