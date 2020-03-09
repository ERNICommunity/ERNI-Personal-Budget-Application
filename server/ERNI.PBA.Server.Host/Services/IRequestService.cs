using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Services
{
    public interface IRequestService
    {
        Task<Transaction[]> CreateTeamTransactions(int userId, decimal amount, CancellationToken cancellationToken);
    }
}
