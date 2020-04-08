using System.Security.Claims;
using ERNI.PBA.Server.Domain.Models;

namespace ERNI.PBA.Server.Domain.Queries.Requests
{
    public class GetRequestQuery : QueryBase<Request>
    {
        public ClaimsPrincipal Principal { get; set; }

        public int RequestId { get; set; }
    }
}
