using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.DataAccess
{
    public interface IUnitOfWork{
        Task SaveChanges(CancellationToken cancellationToken);
    }
}