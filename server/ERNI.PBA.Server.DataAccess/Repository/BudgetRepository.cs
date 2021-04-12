using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using System;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly DatabaseContext _context;

        public BudgetRepository(DatabaseContext context) => _context = context;

        public void AddBudget(Budget budget) => _context.Budgets.Add(budget);

        public async Task AddBudgetAsync(Budget budget) => await _context.Budgets.AddAsync(budget);

        public Task<Budget> GetBudget(int budgetId, CancellationToken cancellationToken)
        {
            return _context.Budgets
                .Include(_ => _.User)
                .Include(_ => _.Transactions)
                .ThenInclude(_ => _.Request)
                .SingleOrDefaultAsync(_ => _.Id == budgetId, cancellationToken);
        }

        public async Task<Budget[]> GetSingleBudgets(Guid userId, int year, CancellationToken cancellationToken)
        {
            return await _context.Budgets
                .Include(_ => _.Transactions).ThenInclude(_ => _.Request)
                .Where(_ => _.BudgetType != BudgetTypeEnum.TeamBudget)
                .Where(_ => _.User.ObjectId == userId && _.Year == year)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<Budget[]> GetTeamBudgets(Guid userId, int year, CancellationToken cancellationToken)
        {
            return await _context.Budgets
                .Include(_ => _.Transactions).ThenInclude(_ => _.Request)
                .Where(_ => _.BudgetType == BudgetTypeEnum.TeamBudget)
                .Where(_ => (_.User.ObjectId == userId || _.User.Superior!.ObjectId == userId) && _.Year == year)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Budget[]> GetBudgets(int userId, int year, CancellationToken cancellationToken)
        {
            return _context.Budgets
                .Include(_ => _.User)
                .Include(_ => _.Transactions).ThenInclude(_ => _.Request)
                .Where(_ => _.UserId == userId && _.Year == year)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Budget[]> GetBudgets(int year, BudgetTypeEnum budgetType, CancellationToken cancellationToken)
        {
            return _context.Budgets
                .Include(_ => _.User)
                .Include(_ => _.Transactions).ThenInclude(_ => _.Request)
                .Where(_ => _.BudgetType == budgetType && _.Year == year)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Budget[]> GetBudgetsByType(int userId, BudgetTypeEnum budgetType, int year, CancellationToken cancellationToken)
        {
            return _context.Budgets
                .Include(_ => _.User)
                .Include(_ => _.Transactions).ThenInclude(_ => _.Request)
                .Where(_ => _.UserId == userId && _.Year == year && _.BudgetType == budgetType)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Budget[]> GetBudgetsByYear(int year, CancellationToken cancellationToken)
        {
            return _context.Budgets
                .Where(_ => _.Year == year)
                .Include(_ => _.User)
                .ToArrayAsync(cancellationToken);
        }

        public Task<decimal> GetTotalRequestedAmount(int budgetId, CancellationToken cancellationToken)
        {
            return _context.Transactions.Where(r => r.BudgetId == budgetId && r.Request.State != RequestState.Rejected)
                .SumAsync(r => r.Amount, cancellationToken);
        }

        public async Task<(int BudgetId, decimal Amount)[]> GetTotalAmountsByYear(int year, CancellationToken cancellationToken)
        {
            return (await _context.Budgets
                .Where(_ => _.Year == year)
                .Select(_ => new
                {
                    BudgetId = _.Id,
                    TotalAmount = _.Transactions
                        .Where(x => x.Request.State != RequestState.Rejected)
                        .Sum(x => x.Amount)
                })
                .ToArrayAsync(cancellationToken))
                .Select(_ => (_.BudgetId, _.TotalAmount))
                .ToArray();
        }

        public Task<Budget[]> GetBudgetsByUser(int userId, CancellationToken cancellationToken) => _context.Budgets.Where(_ => _.UserId == userId).ToArrayAsync(cancellationToken);
    }
}