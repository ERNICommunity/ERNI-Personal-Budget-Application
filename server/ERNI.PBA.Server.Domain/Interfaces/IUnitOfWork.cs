using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChanges(CancellationToken cancellationToken);
    }
}