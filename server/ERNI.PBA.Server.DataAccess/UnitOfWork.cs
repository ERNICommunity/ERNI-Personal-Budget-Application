using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces;

namespace ERNI.PBA.Server.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;

        public UnitOfWork(DatabaseContext context) => _context = context;

        public async Task SaveChanges(CancellationToken cancellationToken) =>
            await _context.SaveChangesAsync(cancellationToken);
    }
}