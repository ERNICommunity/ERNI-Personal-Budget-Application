using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class TeamBudgetFacade : ITeamBudgetFacade
    {
        private readonly DatabaseContext _context;

        public TeamBudgetFacade(DatabaseContext context) => _context = context;

        public async Task<(User Employee, decimal TotalAmount, decimal SpentAmount)[]> GetTeamBudgets(int superiorId, int year, CancellationToken cancellationToken)
        {
            var data = await _context.Budgets
                .Where(b => b.Year == year && b.BudgetType == BudgetTypeEnum.TeamBudget)
                .Where(b => b.UserId == superiorId || b.User.SuperiorId == superiorId)
                .Select(b => new {
                    b.User,
                    TotalAmount = b.Amount,
                    SpentAmount = b.Transactions.Sum(r => r.Amount)
                })
                .ToArrayAsync(cancellationToken);

            return data.Select(d => (d.User, d.TotalAmount, d.SpentAmount)).ToArray();
        }
    }
}
