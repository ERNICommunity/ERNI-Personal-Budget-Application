using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUser(int id, CancellationToken cancellationToken);

        Task<User?> GetUser(Guid id, CancellationToken cancellationToken);

        Task<User?> GetAsync(string username);

        Task<User[]> GetAllUsers(CancellationToken cancellationToken);

        Task<User[]> GetAllUsers(Expression<Func<User, bool>> filter, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the subordinate users for the superior. 
        /// </summary>
        Task<User[]> GetSubordinateUsers(int superiorId, CancellationToken cancellationToken);

        Task AddUserAsync(User user);

        Task<bool> ExistsAsync(string username);

        void Update(User user);
    }
}