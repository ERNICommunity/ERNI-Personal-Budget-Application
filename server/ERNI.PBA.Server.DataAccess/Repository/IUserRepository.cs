using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUser(int id, CancellationToken cancellationToken);

        Task<User[]> GetAllUsers(CancellationToken cancellationToken);

        /// <summary>
        /// Gets the subordinate users for the superior. 
        /// </summary>
        Task<User[]> GetSubordinateUsers(int superiorId, CancellationToken cancellationToken);
    }
}