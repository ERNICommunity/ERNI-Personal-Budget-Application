using MediatR;

namespace ERNI.PBA.Server.Domain.Queries
{
    public abstract class QueryBase<TResult> : IRequest<TResult>
        where TResult : class
    {
    }
}
