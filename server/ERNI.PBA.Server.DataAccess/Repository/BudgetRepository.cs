using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using System;
using System.Linq.Expressions;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class BudgetRepository(DatabaseContext context) : IBudgetRepository
    {
        public async Task AddBudgetAsync(Budget budget) => await context.Budgets.AddAsync(budget);

        public Task<Budget?> GetBudget(int budgetId, CancellationToken cancellationToken) =>
            context.Budgets
                .Include(_ => _.User)
                .Include(_ => _.Transactions)
                .ThenInclude(_ => _.Request)
                .SingleOrDefaultAsync(_ => _.Id == budgetId, cancellationToken);

        public async Task<Budget[]> GetBudgets(int userId, int year, BudgetTypeEnum[] excludedTypes, CancellationToken cancellationToken) =>
            await context.Budgets
                .Include(_ => _.Transactions).ThenInclude(_ => _.Request)
                .Where(_ => !excludedTypes.Contains(_.BudgetType))
                .Where(_ => _.UserId == userId && _.Year == year)
                .ToArrayAsync(cancellationToken);

        public async Task<Budget[]> GetTeamBudgets(Guid userId, int year, CancellationToken cancellationToken)
        {
            return await context.Budgets
                .Include(_ => _.Transactions).ThenInclude(_ => _.Request)
                .Where(_ => _.BudgetType == BudgetTypeEnum.TeamBudget)
                .Where(_ => (_.User.ObjectId == userId || _.User.Superior!.ObjectId == userId) && _.Year == year)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Budget[]> GetBudgets(int userId, int year, CancellationToken cancellationToken)
        {
            return context.Budgets
                .Include(_ => _.User)
                .Include(_ => _.Transactions).ThenInclude(_ => _.Request)
                .Where(_ => _.UserId == userId && _.Year == year)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Budget[]> GetBudgets(int year, BudgetTypeEnum budgetType, CancellationToken cancellationToken)
        {
            return context.Budgets
                .Include(_ => _.User)
                .Include(_ => _.Transactions).ThenInclude(_ => _.Request)
                .Where(_ => _.BudgetType == budgetType && _.Year == year)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Budget[]> GetBudgetsByType(int userId, BudgetTypeEnum budgetType, int year, CancellationToken cancellationToken)
        {
            return context.Budgets
                .Include(_ => _.User)
                .Include(_ => _.Transactions).ThenInclude(_ => _.Request)
                .Where(_ => _.UserId == userId && _.Year == year && _.BudgetType == budgetType)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Budget[]> GetBudgetsByYear(int year, CancellationToken cancellationToken) =>
            context.Budgets
                .Where(_ => _.Year == year)
                .Include(_ => _.User)
                .ToArrayAsync(cancellationToken);

        public Task<decimal> GetTotalRequestedAmount(int budgetId, CancellationToken cancellationToken) =>
            context.Transactions.Where(r => r.BudgetId == budgetId && r.Request.State != RequestState.Rejected)
                .SumAsync(r => r.Amount, cancellationToken);

        public async Task<(Budget Budget, decimal AmountSpent)[]> GetBudgetsWithRequestedAmounts(
            Expression<Func<Budget, bool>> filter, CancellationToken cancellationToken)
        {
            var budgets = await context.Budgets.Where(filter)
                .Select(budget => new { budget, amount = budget.Transactions.Where(t => t.Request.State != RequestState.Rejected).Sum(t => t.Amount) })
                .ToArrayAsync(cancellationToken);

            return budgets.Select(_ => (Budget: _.budget, AmountSpent: _.amount)).ToArray();
        }

        public async Task<(int BudgetId, decimal Amount)[]> GetTotalAmountsByYear(int year, CancellationToken cancellationToken)
        {
            return (await context.Budgets
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

        public async Task<(BudgetTypeEnum type, int count, decimal total, decimal totalSpent)[]> GetBudgetStats(int year)
        {
            return (await context.Budgets.Where(_ => _.Year == year).GroupBy(_ => _.BudgetType)
                .Select(x => new { type = x.Key, count = x.Count(), total = x.Sum(b => b.Amount), spent = x.SelectMany(b => b.Transactions.Select(t => t.Amount)).Sum() })
                .ToArrayAsync())
                .Select(x => (x.type, x.count, x.total, x.spent))
                .ToArray();
        }
    }
}