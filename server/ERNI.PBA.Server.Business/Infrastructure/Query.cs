using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;

namespace ERNI.PBA.Server.Business.Infrastructure
{
    public abstract class Query<T, TResult> : IQuery<T, TResult>
    {
        protected abstract Task<TResult> Execute(T parameter, ClaimsPrincipal principal,
            CancellationToken cancellationToken);

        public async Task<TResult> ExecuteAsync(T parameter, ClaimsPrincipal principal,
            CancellationToken cancellationToken) => await Execute(parameter, principal, cancellationToken);
    }

    public abstract class Query<TResult> : IQuery<TResult>
    {
        protected abstract Task<TResult> Execute(ClaimsPrincipal principal, CancellationToken cancellationToken);

        public async Task<TResult> ExecuteAsync(ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            await Execute(principal, cancellationToken);
    }
}