using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Commands.Requests;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Interfaces.Services;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Host.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Handlers.Requests
{
    public class RejectRequestHandler : IRequestHandler<RejectRequestCommand, bool>
    {
        private readonly IMailService _mailService;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RejectRequestHandler(
            IMailService mailService,
            IRequestRepository requestRepository,
            IUnitOfWork unitOfWork)
        {
            _mailService = mailService;
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RejectRequestCommand command, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(command.RequestId, cancellationToken);
            if (request == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Not a valid id");
            }

            request.State = RequestState.Rejected;

            await _unitOfWork.SaveChanges(cancellationToken);

            _mailService.SendMail("Your request: " + request.Title + " has been " + request.State + ".", request.User.Username);

            return true;
        }
    }
}
