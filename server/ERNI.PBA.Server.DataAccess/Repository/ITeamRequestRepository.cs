using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface ITeamRequestRepository
    {
        Task AddAsync(TeamRequest teamRequest);

        Task<TeamRequest> GetAsync(int requestId);

        Task<TeamRequest[]> GetAllAsync(Expression<Func<TeamRequest, bool>> filter, CancellationToken cancellationToken);

        Task<TeamRequest[]> GetAllByUserAsync(int userId);

        void Delete(TeamRequest teamRequest);
    }
}
