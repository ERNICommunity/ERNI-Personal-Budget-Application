using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Models;

namespace ERNI.PBA.Server.Domain.Interfaces.Repositories
{
    public interface IBudgetRepository
    {
        void AddBudget(Budget budget);

        Task AddBudgetAsync(Budget budget);

        Task<Budget> GetBudget(int budgetId, CancellationToken cancellationToken);

        Task<Budget[]> GetSingleBudgets(int userId, int year, CancellationToken cancellationToken);

        Task<Budget[]> GetTeamBudgets(int userId, int year, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgets(int userId, int year, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgets(int year, BudgetTypeEnum budgetType, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgetsByType(int userId, BudgetTypeEnum budgetType, int year,
            CancellationToken cancellationToken);

        Task<Budget[]> GetBudgetsByUser(int userId, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgetsByYear(int year, CancellationToken cancellationToken);


        Task<(int BudgetId, decimal Amount)[]> GetTotalAmountsByYear(int year, CancellationToken cancellationToken);

        Task<decimal> GetTotalRequestedAmount(int budgetId, CancellationToken cancellationToken);
    }
}