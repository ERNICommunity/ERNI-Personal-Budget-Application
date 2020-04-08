using MediatR;

namespace ERNI.PBA.Server.Domain.Commands
{
    public abstract class CommandBase<T> : IRequest<T>
    {
    }
}
