using MediatR;

namespace ERNI.PBA.Server.Host.Queries
{
    public abstract class QueryBase<TResult> : IRequest<TResult>
        where TResult : class
    {
    }
}
