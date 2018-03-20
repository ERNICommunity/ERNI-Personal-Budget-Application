using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface IBudgetRepository
    {
        Task<Budget> GetBudget(int userId, int year, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgetsByUser(int userId, CancellationToken cancellationToken);

        Task<Budget[]> GetBudgetsByYear(int year, CancellationToken cancellationToken);
    }
}