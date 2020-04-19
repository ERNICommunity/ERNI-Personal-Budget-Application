using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Extensions;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Requests;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Payloads;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Business.Commands.Requests
{
    public class AddTeamRequestCommand : Command<PostRequestModel>, IAddTeamRequestCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddTeamRequestCommand(
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

        protected override async Task Execute(PostRequestModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetId();
            var currentYear = DateTime.Now.Year;
            var currentUser = await _userRepository.GetUser(userId, cancellationToken);
            if (!currentUser.IsSuperior)
            {
                throw AppExceptions.AuthorizationException();
            }

            var budget = await _budgetRepository.GetBudget(parameter.BudgetId, cancellationToken);
            if (budget == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Budget {parameter.BudgetId} was not found.");
            }

            if (budget.BudgetType != BudgetTypeEnum.TeamBudget)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "No Access for request!");
            }

            var teamBudgets = await _budgetRepository.GetTeamBudgets(userId, currentYear, cancellationToken);
            var budgets = teamBudgets.ToTeamBudgets();

            var availableFunds = budgets.Sum(_ => _.Amount);
            if (availableFunds < parameter.Amount)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Requested amount {parameter.Amount} exceeds the limit.");
            }

            var transactions = TransactionCalculator.Create(budgets, parameter.Amount);
            var request = new Request
            {
                BudgetId = budget.Id,
                UserId = userId,
                Year = currentYear,
                Title = parameter.Title,
                Amount = parameter.Amount,
                Date = parameter.Date.ToLocalTime(),
                State = RequestState.Pending,
                Transactions = transactions
            };

            await _requestRepository.AddRequest(request);

            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}
