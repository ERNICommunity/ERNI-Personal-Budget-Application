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

        Task<User> GetUser(string sub, CancellationToken cancellationToken);

        Task<User[]> GetAllUsers(CancellationToken cancellationToken);

        Task<User[]> GetAllUsers(Expression<Func<User, bool>> filter, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the subordinate users for the superior. 
        /// </summary>
        Task<User[]> GetSubordinateUsers(int superiorId, CancellationToken cancellationToken);

        Task<User[]> GetAdminUsers(CancellationToken cancellationToken);

        void AddUser(User user);

        Task AddUserAsync(User user);
    }
}