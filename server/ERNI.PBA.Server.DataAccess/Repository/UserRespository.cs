using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context)
        {
            _context = context;
        }

        public Task<User> GetUser(int id, CancellationToken cancellationToken)
        {
            return _context.Users.Where(_ => _.Id == id)
                .Include(u => u.Superior)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<User[]> GetAllUsers(CancellationToken cancellationToken)
        {
            return _context.Users
                .Include(u => u.Superior)
                .ToArrayAsync(cancellationToken);
        }
        
         public Task<User[]> GetAllUsers(Expression<Func<User, bool>> filter, CancellationToken cancellationToken)
        {
            return _context.Users
                .Where(filter)
                .ToArrayAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the subordinate users for the superior. 
        /// </summary>
        public Task<User[]> GetSubordinateUsers(int superiorId, CancellationToken cancellationToken)
        {
            return _context.Users
                .Include(u => u.Superior)
                .Where(u => u.SuperiorId == superiorId)
                .ToArrayAsync(cancellationToken);
        }

        public Task<User[]> GetAdminUsers(CancellationToken cancellationToken)
        {
            return _context.Users
                .Where(u => u.IsAdmin)
                .ToArrayAsync(cancellationToken);
        }
    }
}