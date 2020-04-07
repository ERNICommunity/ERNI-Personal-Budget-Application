using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Model;
using ERNI.PBA.Server.Host.Model.PendingRequests;

namespace ERNI.PBA.Server.Host.Queries.Requests
{
    public class GetRequestsQuery : QueryBase<RequestModel[]>
    {
        public int UserId { get; set; }

        public int Year { get; set; }

        public IList<RequestState> RequestStates { get; set; }
    }
}
