using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private DatabaseContext _context;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }
        public async Task SaveChanges(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}