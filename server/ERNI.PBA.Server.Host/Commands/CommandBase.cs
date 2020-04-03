using MediatR;

namespace ERNI.PBA.Server.Host.Commands
{
    public abstract class CommandBase<T> : IRequest<T>
        where T : class
    {
    }
}
