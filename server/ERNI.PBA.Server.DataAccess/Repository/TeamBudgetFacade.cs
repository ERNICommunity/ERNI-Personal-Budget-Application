using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.API;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class TeamBudgetFacade : ITeamBudgetFacade
    {
        private readonly DatabaseContext _context;

        public TeamBudgetFacade(DatabaseContext context) => _context = context;

        public Task<Request?> GetTeamRequest(int requestId, CancellationToken cancellationToken) =>
            _context.Requests.Where(_ =>
                    _.Transactions.Any(t => t.Budget.BudgetType == BudgetTypeEnum.TeamBudget) && _.Id == requestId)
                .Include(r => r.Transactions).ThenInclude(t => t.Budget).ThenInclude(r => r.User)
                .SingleOrDefaultAsync(cancellationToken);

        public async Task<(int BudgetId, User Employee, decimal TotalAmount, decimal SpentAmount)[]> GetTeamBudgets(int superiorId, int year, CancellationToken cancellationToken)
        {
            var data = await _context.Budgets
                .Where(b => b.Year == year && b.BudgetType == BudgetTypeEnum.TeamBudget)
                .Where(b => b.UserId == superiorId || b.User.SuperiorId == superiorId)
                .Select(b => new
                {
                    b.Id,
                    b.User,
                    TotalAmount = b.Amount,
                    // Add condition to transactions
                    SpentAmount = b.Transactions.Sum(r => r.Amount)
                })
                .ToArrayAsync(cancellationToken);

            return data.Select(d => (d.Id, d.User, d.TotalAmount, d.SpentAmount)).ToArray();
        }

        public async Task<(int BudgetId, User Employee, decimal TotalAmount, decimal SpentAmount)[]> GetTeamBudgets(int year, CancellationToken cancellationToken)
        {
            var data = await _context.Budgets
                .Where(b => b.Year == year && b.BudgetType == BudgetTypeEnum.TeamBudget)
                .Select(b => new
                {
                    b.Id,
                    b.User,
                    TotalAmount = b.Amount,
                    SpentAmount = b.Transactions.Sum(r => r.Amount)
                })
                .ToArrayAsync(cancellationToken);

            return data.Select(d => (d.Id, d.User, d.TotalAmount, d.SpentAmount)).ToArray();
        }


        public Task<Request[]> GetTeamRequests(int superiorId, int year, CancellationToken cancellationToken) =>
            _context.Requests.Where(_ => _.Transactions.Any(t => t.Budget.BudgetType == BudgetTypeEnum.TeamBudget))
                .Include(r => r.Transactions).ThenInclude(t => t.Budget).ThenInclude(r => r.User)
                .Where(r => r.Year == year && r.UserId == superiorId)
                .ToArrayAsync(cancellationToken);
    }
}
