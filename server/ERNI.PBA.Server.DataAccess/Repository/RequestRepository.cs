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
    }
}