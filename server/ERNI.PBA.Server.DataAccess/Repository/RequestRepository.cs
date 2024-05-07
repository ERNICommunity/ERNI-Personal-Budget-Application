using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class RequestRepository(DatabaseContext context) : IRequestRepository
    {
        public Task<Request[]> GetRequests(Expression<Func<Request, bool>> filter, CancellationToken cancellationToken) =>
            context.Requests
                .Where(filter)
                .Include(_ => _.User.Superior)
                .Include(_ => _.Category)
                .Include(_ => _.Transactions).ThenInclude(_ => _.Budget)
                .OrderByDescending(_ => _.CreateDate)
                .ToArrayAsync(cancellationToken);

        public Task<Transaction[]> GetRequests(int year, int month, BudgetTypeEnum budgetType) =>
            context
                .Transactions
                .Where(_ => ((_.Request.ApprovedDate!.Value.Year == year && _.Request.ApprovedDate.Value.Month == month)
                    ||
                    (_.Request.CompletedDate!.Value.Year == year && _.Request.CompletedDate.Value.Month == month))
                    && _.Budget.BudgetType == budgetType)
                .Include(_ => _.Request)
                .ThenInclude(_ => _.User)
                .ToArrayAsync();

        public Task<Request?> GetRequest(int id, CancellationToken cancellationToken) =>
            context.Requests
                .Include(_ => _.User)
                .Include(_ => _.Transactions)
                .SingleOrDefaultAsync(_ => _.Id == id, cancellationToken);

        public async Task AddRequest(Request request) => await context.Requests.AddAsync(request);

        public async Task AddRequests(IEnumerable<Request> requests) => await context.Requests.AddRangeAsync(requests);

        public async Task DeleteRequest(Request request)
        {
            var transactions = await context.Transactions.Where(_ => _.RequestId == request.Id).ToArrayAsync();
            if (transactions.Length > 0)
            {
                context.Transactions.RemoveRange(transactions);
            }

            context.Requests.Remove(request);
        }

        public async Task AddOrUpdateTransactions(int requestId, IEnumerable<Transaction> transactions)
        {
            var request = await context.Requests.Include(_ => _.Transactions).FirstOrDefaultAsync(_ => _.Id == requestId);
            if (request == null)
            {
                return;
            }

            context.Transactions.RemoveRange(request.Transactions);

            request.Transactions.Clear();
            foreach (var transaction in transactions)
            {
                request.Transactions.Add(transaction);
            }
        }
    }
}