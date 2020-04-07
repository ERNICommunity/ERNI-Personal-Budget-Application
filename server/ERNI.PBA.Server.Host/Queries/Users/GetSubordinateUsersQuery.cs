using System.Collections.Generic;
using System.Security.Claims;
using ERNI.PBA.Server.Host.Model;

namespace ERNI.PBA.Server.Host.Queries.Users
{
    public class GetSubordinateUsersQuery : QueryBase<IEnumerable<UserModel>>
    {
        public ClaimsPrincipal Principal { get; set; }
    }
}
