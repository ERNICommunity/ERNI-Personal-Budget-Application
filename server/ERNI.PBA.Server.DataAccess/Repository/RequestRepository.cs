using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;
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
                .Include(_ => _.Category)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Request[]> GetRequests(Expression<Func<Request, bool>> filter, CancellationToken cancellationToken)
        {
            return _context.Requests
                .Where(filter)
                .Include(_ => _.Budget)
                .ThenInclude(_ => _.User)
                .Include(_ => _.Category)
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

        public void DeleteRequest(Request request)
        {
            _context.Requests.Remove(request);
        }
    }
}