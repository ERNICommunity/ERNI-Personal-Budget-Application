using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Host.Model;

namespace ERNI.PBA.Server.Host.Services
{
    public interface IRequestService
    {
        Task CreateTeamRequests(int userId, TeamRequestInputModel requestInputModel, CancellationToken cancellationToken);
    }
}
