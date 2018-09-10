using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly DatabaseContext _context;

        public BudgetRepository(DatabaseContext context)
        {
            _context = context;
        }

        public void AddBudget(Budget budget)
        {
            _context.Budgets.Add(budget);
        }

        public Task<Budget> GetBudget(int userId, int year, CancellationToken cancellationToken)
        {
            return _context.Budgets.Include(_ => _.User).SingleOrDefaultAsync(_ => _.UserId == userId && _.Year == year, cancellationToken);
        }

        public Task<Budget[]> GetBudgetsByYear(int year, CancellationToken cancellationToken)
        {
            return _context.Budgets
                .Where(_ => _.Year == year)
                .Include(_ => _.User)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Budget[]> GetBudgetsByUser(int userId, CancellationToken cancellationToken)
        {
            return _context.Budgets.Where(_ => _.UserId == userId).ToArrayAsync(cancellationToken);
        }
    }
}