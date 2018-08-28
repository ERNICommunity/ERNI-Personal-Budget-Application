using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUser(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the inferior users for the superior. 
        /// If superior is admin, gets all users.
        /// </summary>
        Task<User[]> GetInferiorUsers(int superiorId, CancellationToken cancellationToken);
    }
}