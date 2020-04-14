using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Responses.PendingRequests;

namespace ERNI.PBA.Server.Domain.Interfaces.Queries.Requests
{
    public interface IGetRequestsQuery : IQuery<GetRequestsModel, RequestModel[]>
    {
    }
}
