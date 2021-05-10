using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Domain.Interfaces.Repositories
{
    public interface ITeamBudgetFacade
    {
        Task<Request[]> GetTeamRequests(int superiorId, int year, CancellationToken cancellationToken);

        Task<Request> GetTeamRequest(int requestId, CancellationToken cancellationToken);

        Task<(User Employee, decimal TotalAmount, decimal SpentAmount)[]> GetTeamBudgets(int superiorId, int year,
            CancellationToken cancellationToken);

        Task<(User Employee, decimal TotalAmount, decimal SpentAmount)[]> GetTeamBudgets(int year,
            CancellationToken cancellationToken);
    }
}
