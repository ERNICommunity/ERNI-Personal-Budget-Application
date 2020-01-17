using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface ITeamRequestRepository
    {
        Task AddAsync(TeamRequest teamRequest);

        Task<TeamRequest[]> GetAllAsync(int userId);
    }
}
