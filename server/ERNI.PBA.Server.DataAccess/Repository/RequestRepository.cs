using ERNI.PBA.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

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
            var transactions = await _context.Transactions.Where(_ => _.RequestId == request.Id).ToArrayAsync();
            if (transactions.Any())
                _context.Transactions.RemoveRange(transactions);

            _context.Requests.Remove(request);
        }

        public async Task AddOrUpdateTransactions(int requestId, Transaction[] transactions)
        {
            var request = await _context.Requests.Include(_ => _.Transactions).FirstOrDefaultAsync(_ => _.Id == requestId);
            if (request == null)
                return;

            _context.Transactions.RemoveRange(request.Transactions);

            request.Transactions.Clear();
            foreach (var transaction in transactions)
            {
                request.Transactions.Add(transaction);
            }
        }
    }
}