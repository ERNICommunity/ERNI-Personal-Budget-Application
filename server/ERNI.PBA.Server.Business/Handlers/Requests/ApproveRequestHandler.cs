using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Commands.Requests;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Business.Handlers.Requests
{
    public class ApproveRequestHandler : IRequestHandler<ApproveRequestCommand, bool>
    {
        private readonly IMailService _mailService;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public ApproveRequestHandler(
            IMailService mailService,
            IRequestRepository requestRepository,
            IUnitOfWork unitOfWork,
            ILogger<ApproveRequestHandler> logger)
        {
            _mailService = mailService;
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(ApproveRequestCommand command, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(command.RequestId, cancellationToken);
            if (request == null)
            {
                _logger.LogWarning("Not a valid id");
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Not a valid id");
            }

            request.State = RequestState.Approved;

            await _unitOfWork.SaveChanges(cancellationToken);

            var message = "Request: " + request.Title + " of amount: " + request.Amount + " has been " +
                          request.State + ".";

            _mailService.SendMail(message, request.User.Username);

            return true;
        }
    }
}
