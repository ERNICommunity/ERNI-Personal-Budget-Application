using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface IRequestRepository
    {
        Task<Request[]> GetRequests(int year, int userId, CancellationToken cancellationToken);

        Task<Request[]> GetRequests(Expression<Func<Request, bool>> filter, CancellationToken cancellationToken);

        Task<Request> GetRequest(int id, CancellationToken cancellationToken);
        void AddRequest(Request request);
    }
}