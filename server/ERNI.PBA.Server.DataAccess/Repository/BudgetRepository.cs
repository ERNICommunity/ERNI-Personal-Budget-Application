using ERNI.PBA.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task AddBudgetAsync(Budget budget)
        {
            await _context.Budgets.AddAsync(budget);
        }

        public async Task<Budget[]> GetTeamBudgets(int userId, int year, CancellationToken cancellationToken)
        {
            return await _context.Budgets
                .Include(_ => _.User)
                .Include(_ => _.Requests)
                .Where(_ => _.User.Id == userId || _.User.SuperiorId == userId)
                .Where(_ => _.BudgetType == BudgetTypeEnum.TeamBudget && _.Year == year)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Budget[]> GetSingleBudgets(int userId, int year, CancellationToken cancellationToken)
        {
            return _context.Budgets
                .Include(_ => _.User)
                .Include(_ => _.Requests).ThenInclude(_ => _.Category)
                .Where(_ => _.BudgetType != BudgetTypeEnum.TeamBudget)
                .Where(_ => _.UserId == userId && _.Year == year)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Budget> GetBudget(int budgetId, CancellationToken cancellationToken)
        {
            return _context.Budgets
                .Include(_ => _.User)
                .Include(_ => _.Requests).ThenInclude(_ => _.Category)
                .SingleOrDefaultAsync(_ => _.Id == budgetId, cancellationToken);
        }

        public Task<Budget[]> GetBudgets(int userId, int year, CancellationToken cancellationToken)
        {
            return _context.Budgets
                .Include(_ => _.User)
                .Include(_ => _.Requests).ThenInclude(_ => _.Category)
                .Where(_ => _.UserId == userId && _.Year == year)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Budget[]> GetBudgets(int year, BudgetTypeEnum budgetType, CancellationToken cancellationToken)
        {
            return _context.Budgets
                .Include(_ => _.User)
                .Include(_ => _.Requests).ThenInclude(_ => _.Category)
                .Where(_ => _.BudgetType == budgetType && _.Year == year)
                .ToArrayAsync(cancellationToken);
        }

        public Task<Budget[]> GetBudgetsByType(int userId, BudgetTypeEnum budgetType, int year, CancellationToken cancellationToken)
        {
            return _context.Budgets
                .Include(_ => _.User)
                .Include(_ => _.Requests).ThenInclude(_ => _.Category)
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
            return _context.Budgets.Where(_ => _.Id == budgetId)
                .Select(_ => _.Requests.Sum(r => r.Amount))
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<(int BudgetId, decimal Amount)[]> GetTotalAmountsByYear(int year, CancellationToken cancellationToken)
        {
            return (await _context.Budgets
                .Where(_ => _.Year == year)
                .Select(_ => new
                {
                    BudgetId = _.Id,
                    TotalAmount = _.Requests.Sum(r => r.Amount)
                })
                .ToArrayAsync(cancellationToken))
                .Select(_ => (BudgetId: _.BudgetId, TotalAmount: _.TotalAmount))
                .ToArray();
        }

        public Task<Budget[]> GetBudgetsByUser(int userId, CancellationToken cancellationToken)
        {
            return _context.Budgets.Where(_ => _.UserId == userId).ToArrayAsync(cancellationToken);
        }
    }
}