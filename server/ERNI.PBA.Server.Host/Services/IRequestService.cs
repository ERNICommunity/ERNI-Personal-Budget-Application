using System.Threading.Tasks;

namespace ERNI.PBA.Server.Host.Services
{
    public interface IRequestService
    {
        Task CreateTeamRequests(int userId);
    }
}
