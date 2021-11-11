using System;
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

        Task<Budget[]> GetSingleBudgets(Guid userId, int year, CancellationToken cancellationToken);

        Task<Budget[]> GetTeamBudgets(Guid userId, int year, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgets(int userId, int year, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgets(int year, BudgetTypeEnum budgetType, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgetsByType(int userId, BudgetTypeEnum budgetType, int year,
            CancellationToken cancellationToken);

        Task<Budget[]> GetBudgetsByYear(int year, CancellationToken cancellationToken);

        Task<(int BudgetId, decimal Amount)[]> GetTotalAmountsByYear(int year, CancellationToken cancellationToken);

        Task<decimal> GetTotalRequestedAmount(int budgetId, CancellationToken cancellationToken);
    }
}