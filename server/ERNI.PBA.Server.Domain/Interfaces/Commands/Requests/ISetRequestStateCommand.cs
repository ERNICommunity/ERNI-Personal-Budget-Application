using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;

namespace ERNI.PBA.Server.Domain.Interfaces.Commands.Requests
{
    public interface ISetRequestStateCommand : ICommand<(int requestId, RequestState requestState)>
    {
    }
}
