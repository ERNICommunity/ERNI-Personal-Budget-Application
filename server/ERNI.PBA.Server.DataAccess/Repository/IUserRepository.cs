using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUser(int id, CancellationToken cancellationToken);

        Task<User[]> GetAllUsers(CancellationToken cancellationToken);
        
        Task<User[]> GetAllUsers(Expression<Func<User, bool>> filter, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the subordinate users for the superior. 
        /// </summary>
        Task<User[]> GetSubordinateUsers(int superiorId, CancellationToken cancellationToken);

        Task<User[]> GetAdminUsers(CancellationToken cancellationToken);

        /// <summary>
        /// Adds user to the repository. Returns true if user was newly added, or false if user already exists.
        /// </summary>
        Task<bool> AddUser(User user);
    }
}