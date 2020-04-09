using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.Domain.Interfaces.Infrastructure
{
    public interface IQuery<in TInput, TOutput>
    {
        Task<TOutput> ExecuteAsync(TInput parameter, ClaimsPrincipal principal, CancellationToken cancellationToken);
    }
}
