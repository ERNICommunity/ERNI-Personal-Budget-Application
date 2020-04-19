using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Requests;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Business.Commands.Requests
{
    public class ApproveRequestCommand : Command<int>, IApproveRequestCommand
    {
        private readonly IMailService _mailService;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public ApproveRequestCommand(
            IMailService mailService,
            IRequestRepository requestRepository,
            IUnitOfWork unitOfWork,
            ILogger<ApproveRequestCommand> logger)
        {
            _mailService = mailService;
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        protected override async Task Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(parameter, cancellationToken);
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
        }
    }
}
