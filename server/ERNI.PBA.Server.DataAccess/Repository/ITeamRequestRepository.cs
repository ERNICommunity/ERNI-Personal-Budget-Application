using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface ITeamRequestRepository
    {
        Task AddAsync(TeamRequest teamRequest);

        Task<TeamRequest> GetAsync(int requestId);

        Task<TeamRequest[]> GetAllAsync(int userId);

        void Remove(TeamRequest teamRequest);
    }
}
