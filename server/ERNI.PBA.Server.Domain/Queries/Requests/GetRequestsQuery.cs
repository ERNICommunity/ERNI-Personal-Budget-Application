using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Models.Outputs.PendingRequests;

namespace ERNI.PBA.Server.Domain.Queries.Requests
{
    public class GetRequestsQuery : QueryBase<RequestModel[]>
    {
        public int UserId { get; set; }

        public int Year { get; set; }

        public IList<RequestState> RequestStates { get; set; }
    }
}
