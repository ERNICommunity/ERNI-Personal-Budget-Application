using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Host.Commands.Requests;
using ERNI.PBA.Server.Host.Exceptions;
using ERNI.PBA.Server.Host.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Handlers.Requests
{
    public class DeleteRequestHandler : IRequestHandler<DeleteRequestCommand, bool>
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRequestHandler(
            IRequestRepository requestRepository,
            IUnitOfWork unitOfWork)
        {
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteRequestCommand command, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(command.RequestId, cancellationToken);
            if (request == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Not a valid id");
            }

            if (!command.Principal.IsInRole(Roles.Admin) && command.Principal.GetId() != request.UserId)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Access denied");
            }

            await _requestRepository.DeleteRequest(request);

            await _unitOfWork.SaveChanges(cancellationToken);

            return true;
        }
    }
}
