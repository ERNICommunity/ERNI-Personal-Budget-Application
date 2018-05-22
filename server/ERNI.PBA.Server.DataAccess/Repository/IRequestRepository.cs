using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface IRequestRepository
    {
        Task<Request[]> GetRequests(int year, int userId, CancellationToken cancellationToken);

        Task<Request[]> GetPendingRequests(CancellationToken cancellationToken);
    }
}