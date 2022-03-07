using System.Linq;
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
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Payloads;

namespace ERNI.PBA.Server.Business.Commands.Requests
{
    public class UpdateRequestCommand : Command<UpdateRequestModel>, IUpdateRequestCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRequestCommand(
            IUserRepository userRepository,
            IRequestRepository requestRepository,
            IBudgetRepository budgetRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _budgetRepository = budgetRepository;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Execute(UpdateRequestModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(parameter.Id, cancellationToken);
            if (request == null)
            {
                throw new OperationErrorException(ErrorCodes.RequestNotFound, $"Request with id {parameter.Id} not found.");
            }

            var currentUser = await _userRepository.GetUser(principal.GetId(), cancellationToken)
                ?? throw AppExceptions.AuthorizationException();

            if (currentUser.Id != request.User.Id)
            {
                throw new OperationErrorException(ErrorCodes.AccessDenied, "No Access for request!");
            }

            if (request.State != RequestState.Pending)
            {
                throw new OperationErrorException(ErrorCodes.CannotUpdateRequest, "Only pending requests can be updated.");
            }

            var budgetId = request.Transactions.First().BudgetId;

            var requestedAmount = await _budgetRepository.GetTotalRequestedAmount(budgetId, cancellationToken);

            var budget = await _budgetRepository.GetBudget(budgetId, cancellationToken);
            if (budget == null || budget.BudgetType == BudgetTypeEnum.TeamBudget)
            {
                throw new OperationErrorException(ErrorCodes.AccessDenied, "No Access for request!");
            }

            if (parameter.Amount > budget.Amount + request.Amount - requestedAmount)
            {
                throw new OperationErrorException(ErrorCodes.InvalidAmount, $"Requested amount {parameter.Amount} exceeds the amount left ({requestedAmount} of {budget.Amount}).");
            }

            request.Title = parameter.Title;
            request.Amount = parameter.Amount;

            var transactions = new[]
            {
                new Transaction
                {
                    RequestId = request.Id,
                    BudgetId = budget.Id,
                    Amount = request.InvoicedAmount ?? parameter.Amount,
                    RequestType = budget.BudgetType,
                }
            };
            await _requestRepository.AddOrUpdateTransactions(request.Id, transactions);

            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}
