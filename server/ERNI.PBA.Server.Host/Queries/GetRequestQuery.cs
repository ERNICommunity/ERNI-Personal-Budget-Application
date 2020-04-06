using System.Security.Claims;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Queries
{
    public class GetRequestQuery : QueryBase<Request>
    {
        public ClaimsPrincipal Principal { get; set; }

        public int RequestId { get; set; }
    }
}
