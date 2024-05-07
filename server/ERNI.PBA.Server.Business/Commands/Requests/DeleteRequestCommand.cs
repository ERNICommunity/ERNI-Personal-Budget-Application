using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Requests;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Security;

namespace ERNI.PBA.Server.Business.Commands.Requests;

public class DeleteRequestCommand(
    IRequestRepository requestRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : Command<int>, IDeleteRequestCommand
{
    protected override async Task Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        var request = await requestRepository.GetRequest(parameter, cancellationToken) ?? throw new OperationErrorException(ErrorCodes.RequestNotFound, "Not a valid id");

        var user = await userRepository.GetUser(principal.GetId(), cancellationToken);

        if (!principal.IsInRole(Roles.Admin) && user?.Id != request.UserId)
        {
            throw new OperationErrorException(ErrorCodes.AccessDenied, "Access denied");
        }

        if (request.State == RequestState.Approved)
        {
            throw new OperationErrorException(ErrorCodes.CannotDeleteCompletedRequest, "Cannot delete approved request");
        }

        await requestRepository.DeleteRequest(request);

        await unitOfWork.SaveChanges(cancellationToken);
    }
}
