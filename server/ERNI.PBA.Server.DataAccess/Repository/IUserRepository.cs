using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUser(int id);

        Task<User[]> GetUsers(CancellationToken cancellationToken);
    }
}