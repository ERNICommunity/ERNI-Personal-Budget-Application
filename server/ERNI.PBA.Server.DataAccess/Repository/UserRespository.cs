using System.Linq;
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

        /// <summary>
        /// Gets the inferior users for the superior. 
        /// If superior is admin, gets all users.
        /// </summary>
        public Task<User[]> GetInferiorUsers(int superiorId, CancellationToken cancellationToken)
        {
            var user = GetUser(superiorId, cancellationToken).Result;

            var users = _context.Users.Include(u => u.Superior);

            if (user.IsAdmin)
            {
                return users.ToArrayAsync(cancellationToken);
            }

            return users
                .Where(u => u.SuperiorId == superiorId)
                .ToArrayAsync(cancellationToken);
        }
    }
}