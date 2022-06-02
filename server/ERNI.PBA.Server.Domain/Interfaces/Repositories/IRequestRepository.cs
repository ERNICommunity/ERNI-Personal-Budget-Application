using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.API;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Domain.Interfaces.Repositories
{
    public interface IRequestRepository
    {
        Task<Request[]> GetRequests(Expression<Func<Request, bool>> filter, CancellationToken cancellationToken);

        Task<Request?> GetRequest(int id, CancellationToken cancellationToken);

        Task AddRequest(Request request);

        Task AddRequests(IEnumerable<Request> requests);

        Task<Transaction[]> GetRequests(int year, int month, BudgetTypeEnum budgetType);

        Task DeleteRequest(Request request);

        Task AddOrUpdateTransactions(int requestId, IEnumerable<Transaction> transactions);
    }
}