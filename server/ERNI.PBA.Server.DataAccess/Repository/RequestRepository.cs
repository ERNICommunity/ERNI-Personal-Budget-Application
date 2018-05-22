using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class RequestRepository : IRequestRepository
    {
        private readonly DatabaseContext _context;

        public RequestRepository(DatabaseContext context)
        {
            _context = context;
        }

        public Task<Request[]> GetRequests(int year, int userId, CancellationToken cancellationToken)
        {
            return _context.Requests
                .Where(_ => _.Year == year && _.UserId == userId)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Request[]> GetPendingRequests(CancellationToken cancellationToken)
        {
            return _context.Requests
                .Where(_ => _.State != RequestState.Approved && _.State != RequestState.Rejected)
                .Include(_ => _.Budget)
                .ThenInclude(_ => _.User)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Request> GetRequest(int id, CancellationToken cancellationToken)
        {
            return _context.Requests.SingleOrDefaultAsync(_ => _.Id == id, cancellationToken);
        }

        public void AddRequest(Request request)
        {
            _context.Requests.Add(request);
        }
    }
}