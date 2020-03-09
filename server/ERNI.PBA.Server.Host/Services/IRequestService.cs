using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.Host.Model;

namespace ERNI.PBA.Server.Host.Services
{
    public interface IRequestService
    {
        Task<Transaction[]> CreateTeamTransactions(int userId, PostRequestModel postRequestModel, CancellationToken cancellationToken);
    }
}
