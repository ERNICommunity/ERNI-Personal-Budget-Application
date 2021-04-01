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
using ERNI.PBA.Server.Domain.Models.Payloads;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Business.Commands.Requests
{
    public class SetInvoicedAmountCommand : Command<(int requestId, SetInvoicedAmountModel model)>, ISetInvoicedAmountCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public SetInvoicedAmountCommand(
            IUserRepository userRepository,
            IRequestRepository requestRepository,
            IUnitOfWork unitOfWork,
            ILogger<SetInvoicedAmountCommand> logger)
        {
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        protected override async Task Execute((int requestId, SetInvoicedAmountModel model) parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(parameter.requestId, cancellationToken);
            if (request == null)
            {
                _logger.LogWarning("Not a valid id");
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Not a valid id");
            }

            var currentUser = await _userRepository.GetUser(principal.GetId(), cancellationToken);
            if (currentUser.Id != request.User.Id)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "No Access for request!");
            }

            if (request.State != RequestState.Approved)
            {
                _logger.LogWarning("Validation failed");
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Validation failed");
            }

            if (parameter.model.Amount > request.Amount)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Invoiced amount {parameter.model.Amount} exceeds the approved amount of {request.Amount}.");
            }

            request.InvoicedAmount = parameter.model.Amount;

            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}