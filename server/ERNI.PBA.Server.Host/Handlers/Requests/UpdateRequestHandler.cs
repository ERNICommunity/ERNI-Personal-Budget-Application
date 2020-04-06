using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Commands.Requests;
using ERNI.PBA.Server.Host.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Handlers.Requests
{
    public class UpdateRequestHandler : IRequestHandler<UpdateRequestCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRequestHandler(
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

        public async Task<bool> Handle(UpdateRequestCommand command, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(command.RequestId, cancellationToken);
            if (request == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Request with id {command.RequestId} not found.");
            }

            var currentUser = await _userRepository.GetUser(command.UserId, cancellationToken);
            if (currentUser.Id != request.User.Id)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "No Access for request!");
            }

            var requestedAmount = await _budgetRepository.GetTotalRequestedAmount(request.BudgetId, cancellationToken);

            var budget = await _budgetRepository.GetBudget(request.BudgetId, cancellationToken);
            if (budget.BudgetType == BudgetTypeEnum.TeamBudget)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "No Access for request!");
            }

            if (command.Amount > budget.Amount + request.Amount - requestedAmount)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Requested amount {command.Amount} exceeds the amount left ({requestedAmount} of {budget.Amount}).");
            }

            request.Title = command.Title;
            request.Amount = command.Amount;
            request.Date = command.Date.ToLocalTime();

            var transactions = new[]
            {
                new Transaction
                {
                    RequestId = request.Id,
                    BudgetId = budget.Id,
                    UserId = currentUser.Id,
                    Amount = command.Amount
                }
            };
            await _requestRepository.AddOrUpdateTransactions(request.Id, transactions);

            await _unitOfWork.SaveChanges(cancellationToken);

            return true;
        }
    }
}
