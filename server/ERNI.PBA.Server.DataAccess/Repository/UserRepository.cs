using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class UserRepository(DatabaseContext context) : IUserRepository
    {
        public Task<User?> GetUser(int id, CancellationToken cancellationToken) =>
            context.Users.Where(_ => _.Id == id)
                .Include(u => u.Superior)
                .FirstOrDefaultAsync(cancellationToken);

        public Task<User?> GetUser(Guid id, CancellationToken cancellationToken) =>
            context.Users.Where(_ => _.ObjectId == id)
                .Include(u => u.Superior)
                .FirstOrDefaultAsync(cancellationToken);

        public Task<User?> GetAsync(string username) => context.Users.SingleOrDefaultAsync(x => x.Username == username);

        public Task<User[]> GetAllUsers(CancellationToken cancellationToken)
        {
            return context.Users
                .Include(u => u.Superior)
                .ToArrayAsync(cancellationToken);
        }

        public Task<User[]> GetAllUsers(Expression<Func<User, bool>> filter, CancellationToken cancellationToken)
        {
            return context.Users
                .Where(filter)
                .ToArrayAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the subordinate users for the superior. 
        /// </summary>
        public Task<User[]> GetSubordinateUsers(int superiorId, CancellationToken cancellationToken)
        {
            return context.Users
                .Include(u => u.Superior)
                .Where(u => u.SuperiorId == superiorId)
                .ToArrayAsync(cancellationToken);
        }

        public async Task AddUserAsync(User user) => await context.Users.AddAsync(user);

        public async Task<bool> ExistsAsync(string username) => await context.Users.AnyAsync(x => x.Username == username);

        public void Update(User user) => context.Entry(user).State = EntityState.Modified;
    }
}