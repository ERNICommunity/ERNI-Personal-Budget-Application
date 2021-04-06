using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Domain.Interfaces.Repositories
{
    public interface ITeamBudgetFacade
    {
        Task<(User Employee, decimal TotalAmount, decimal SpentAmount)[]> GetTeamBudgets(int superiorId, int year,
            CancellationToken cancellationToken);
    }
}
