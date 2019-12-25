using System;
using System.Collections;
using System.Collections.Generic;
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

        public Task<Request[]> GetRequests(int budgetId, CancellationToken cancellationToken)
        {
            return _context.Requests
                .Where(_ => _.BudgetId == budgetId)
                .Include(_ => _.Category)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Request[]> GetRequests(Expression<Func<Request, bool>> filter, CancellationToken cancellationToken)
        {
            return _context.Requests
                .Where(filter)
                .Include(_ => _.Budget)
                .Include(_ => _.User.Superior)
                .Include(_ => _.Category)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Request> GetRequest(int id, CancellationToken cancellationToken)
        {
            return _context.Requests
                .Include(_ => _.User)
                .Include(_ => _.Category)
                .SingleOrDefaultAsync(_ => _.Id == id, cancellationToken);
        }

        public async Task AddRequest(Request request)
        {
            await _context.Requests.AddAsync(request);
        }

        public async Task AddRequests(IEnumerable<Request> requests)
        {
            await _context.Requests.AddRangeAsync(requests);
        }

        public async Task DeleteRequest(Request request)
        {
            _context.Requests.Remove(request);
        }
    }
}