using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Commands.Requests;
using ERNI.PBA.Server.Host.Exceptions;
using ERNI.PBA.Server.Host.Services;
using ERNI.PBA.Server.Host.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Handlers.Requests
{
    public class AddTeamRequestHandler : IRequestHandler<AddTeamRequestCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddTeamRequestHandler(
            IUserRepository userRepository,
            IBudgetRepository budgetRepository,
            IRequestRepository requestRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _budgetRepository = budgetRepository;
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(AddTeamRequestCommand command, CancellationToken cancellationToken)
        {
            var currentUser = await _userRepository.GetUser(command.UserId, cancellationToken);
            if (!currentUser.IsSuperior)
            {
                throw AppExceptions.AuthorizationException();
            }

            var budget = await _budgetRepository.GetBudget(command.BudgetId, cancellationToken);
            if (budget == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Budget {command.BudgetId} was not found.");
            }

            if (budget.BudgetType != BudgetTypeEnum.TeamBudget)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "No Access for request!");
            }

            var teamBudgets = await _budgetRepository.GetTeamBudgets(command.UserId, command.CurrentYear, cancellationToken);
            var budgets = teamBudgets.ToTeamBudgets();

            var availableFunds = budgets.Sum(_ => _.Amount);
            if (availableFunds < command.Amount)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Requested amount {command.Amount} exceeds the limit.");
            }

            var transactions = TransactionCalculator.Create(budgets, command.Amount);
            var request = new Request
            {
                BudgetId = budget.Id,
                UserId = command.UserId,
                Year = command.CurrentYear,
                Title = command.Title,
                Amount = command.Amount,
                Date = command.Date.ToLocalTime(),
                State = RequestState.Pending,
                Transactions = transactions
            };

            await _requestRepository.AddRequest(request);

            await _unitOfWork.SaveChanges(cancellationToken);

            return true;
        }
    }
}
