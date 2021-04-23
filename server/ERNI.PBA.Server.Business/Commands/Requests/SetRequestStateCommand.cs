using System;
using System.Linq;
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
    public class SetRequestStateCommand : Command<(int requestId, RequestState requestState)>, ISetRequestStateCommand
    {
        private readonly IMailService _mailService;
        private readonly IRequestRepository _requestRepository;
        private readonly IInvoiceImageRepository _invoiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public SetRequestStateCommand(
            IMailService mailService,
            IRequestRepository requestRepository,
            IInvoiceImageRepository invoiceRepository,
            IUnitOfWork unitOfWork,
            ILogger<SetRequestStateCommand> logger)
        {
            _mailService = mailService;
            _requestRepository = requestRepository;
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        protected override async Task Execute((int requestId, RequestState requestState) parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(parameter.requestId, cancellationToken);
            if (request == null)
            {
                _logger.LogWarning("Not a valid id");
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Not a valid id");
            }

            var invoiceCount = (await _invoiceRepository.GetInvoiceCounts(new[] {request.Id}, cancellationToken))
                .SingleOrDefault().invoiceCount;

            var validationResult = Validate(parameter.requestState, request, invoiceCount);
            if (!validationResult.isvalid)
            {
                _logger.LogWarning(validationResult.error);
                throw new OperationErrorException(StatusCodes.Status400BadRequest, validationResult.error!);
            }

            request.State = parameter.requestState;

            if (parameter.requestState == RequestState.Completed)
            {
                request.ApprovedDate = DateTime.Now;
            }

            await _unitOfWork.SaveChanges(cancellationToken);

            var message = "Request: " + request.Title + " of amount: " + request.Amount + " has been " +
                          request.State + ".";
            _mailService.SendMail("Your request: " + request.Title + " has been " + request.State + ".", request.User.Username);

            _mailService.SendMail(message, request.User.Username);
        }

        private static (bool isvalid, string? error) Validate(RequestState newState, Domain.Models.Entities.Request request, int invoiceCount) => newState switch
        {
            RequestState.Pending => (false, "Cannot change state to pending"),
            RequestState.Approved => request.State == RequestState.Pending ? (true, null) : (false, "Only pending requests can be approved."),
            RequestState.Completed => CanComplete(request, invoiceCount),
            RequestState.Rejected => (true, null),
            _ => throw new NotImplementedException(),
        };

        private static (bool isValid, string? error) CanComplete(Domain.Models.Entities.Request request,
            int invoiceCount)
        {
            if (request.State != RequestState.Approved)
            {
                return (false, "The request is not approved.");
            }

            if (!request.InvoicedAmount.HasValue || request.InvoicedAmount <= 0)
            {
                return (false, "Request without invoiced amount cannot be completed.");
            }

            if (invoiceCount <= 0)
            {
                return (false, "The request has no invoices attached.");
            }

            return (true, null);
        }
    }
}