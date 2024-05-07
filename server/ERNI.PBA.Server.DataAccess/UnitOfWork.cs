using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces;

namespace ERNI.PBA.Server.DataAccess
{
    public class UnitOfWork(DatabaseContext context) : IUnitOfWork
    {
        public async Task SaveChanges(CancellationToken cancellationToken) =>
            await context.SaveChangesAsync(cancellationToken);
    }
}