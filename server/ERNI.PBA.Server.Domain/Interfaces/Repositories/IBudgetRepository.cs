using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Domain.Interfaces.Repositories
{
    public interface IBudgetRepository
    {
        Task AddBudgetAsync(Budget budget);

        Task<Budget?> GetBudget(int budgetId, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgets(int userId, int year, BudgetTypeEnum[] excludedTypes,
            CancellationToken cancellationToken);

        Task<Budget[]> GetTeamBudgets(Guid userId, int year, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgets(int userId, int year, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgets(int year, BudgetTypeEnum budgetType, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgetsByType(int userId, BudgetTypeEnum budgetType, int year,
            CancellationToken cancellationToken);

        Task<Budget[]> GetBudgetsByYear(int year, CancellationToken cancellationToken);

        Task<(int BudgetId, decimal Amount)[]> GetTotalAmountsByYear(int year, CancellationToken cancellationToken);

        Task<decimal> GetTotalRequestedAmount(int budgetId, CancellationToken cancellationToken);

        Task<(Budget Budget, decimal AmountSpent)[]> GetBudgetsWithRequestedAmounts(Expression<Func<Budget, bool>> filter, CancellationToken cancellationToken);

        Task<(BudgetTypeEnum type, int count, decimal total, decimal totalSpent)[]> GetBudgetStats(int year);
    }
}