using ERNI.PBA.Server.Business.Extensions;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Requests;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Payloads;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.Business.Commands.Requests
{
    public class UpdateTeamRequestCommand : Command<UpdateRequestModel>, IUpdateTeamRequestCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTeamRequestCommand(
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
            var userId = principal.GetId();

            var request = await _requestRepository.GetRequest(parameter.Id, cancellationToken);
            if (request == null)
            {
                throw new OperationErrorException(ErrorCodes.RequestNotFound, $"Request with id {parameter.Id} not found.");
            }

            var user = await _userRepository.GetUser(principal.GetId(), cancellationToken);

            if (user.Id != request.UserId)
            {
                throw new OperationErrorException(ErrorCodes.AccessDenied, "No Access for request!");
            }

            var teamBudgets = await _budgetRepository.GetTeamBudgets(userId, DateTime.Now.Year, cancellationToken);
            if (teamBudgets.Any(x => x.BudgetType != BudgetTypeEnum.TeamBudget))
            {
                throw new OperationErrorException(ErrorCodes.AccessDenied, "No Access for request!");
            }

            var budgets = teamBudgets.ToTeamBudgets(x => x.RequestId != parameter.Id);

            var availableFunds = budgets.Sum(_ => _.Amount);
            if (availableFunds < parameter.Amount)
            {
                throw new OperationErrorException(ErrorCodes.InvalidAmount, $"Requested amount {parameter.Amount} exceeds the limit.");
            }

            var transactions = TransactionCalculator.Create(budgets, parameter.Amount);
            request.Title = parameter.Title;
            request.Amount = parameter.Amount;
            request.Date = parameter.Date.ToLocalTime();
            await _requestRepository.AddOrUpdateTransactions(request.Id, transactions);

            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}