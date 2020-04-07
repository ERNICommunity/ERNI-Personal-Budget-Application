using System.Security.Claims;
using ERNI.PBA.Server.Domain.Entities;

namespace ERNI.PBA.Server.Host.Queries.Requests
{
    public class GetRequestQuery : QueryBase<Request>
    {
        public ClaimsPrincipal Principal { get; set; }

        public int RequestId { get; set; }
    }
}
