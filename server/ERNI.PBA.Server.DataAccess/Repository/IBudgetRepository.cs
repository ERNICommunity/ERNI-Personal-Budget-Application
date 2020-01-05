using ERNI.PBA.Server.DataAccess.Model;
using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface IBudgetRepository
    {
        void AddBudget(Budget budget);

        Task AddBudgetAsync(Budget budget);

        Task<Budget> GetBudget(int budgetId, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgets(int userId, int year, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgetsByType(int userId, BudgetTypeEnum budgetType, int year,
            CancellationToken cancellationToken);

        Task<(int BudgetId, decimal Amount)[]> GetTotalAmountsByYear(int year, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgetsByUser(int userId, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgetsByYear(int year, CancellationToken cancellationToken);

        Task<decimal> GetTotalRequestedAmount(int budgetId, CancellationToken cancellationToken);
    }
}