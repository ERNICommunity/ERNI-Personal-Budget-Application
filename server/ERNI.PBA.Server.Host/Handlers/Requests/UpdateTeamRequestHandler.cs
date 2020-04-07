using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Model;
using ERNI.PBA.Server.Host.Commands.Requests;
using ERNI.PBA.Server.Host.Exceptions;
using ERNI.PBA.Server.Host.Services;
using ERNI.PBA.Server.Host.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Handlers.Requests
{
    public class UpdateTeamRequestHandler : IRequestHandler<UpdateTeamRequestCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTeamRequestHandler(
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

        public async Task<bool> Handle(UpdateTeamRequestCommand command, CancellationToken cancellationToken)
        {
            var currentUser = await _userRepository.GetUser(command.UserId, cancellationToken);
            if (!currentUser.IsSuperior)
            {
                throw AppExceptions.AuthorizationException();
            }

            var request = await _requestRepository.GetRequest(command.RequestId, cancellationToken);
            if (request == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Request with id {command.RequestId} not found.");
            }

            if (command.UserId != request.UserId)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "No Access for request!");
            }

            var teamBudgets = await _budgetRepository.GetTeamBudgets(command.UserId, DateTime.Now.Year, cancellationToken);
            if (teamBudgets.Any(x => x.BudgetType != BudgetTypeEnum.TeamBudget))
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "No Access for request!");
            }

            var budgets = teamBudgets.ToTeamBudgets(x => x.RequestId != command.RequestId);

            var availableFunds = budgets.Sum(_ => _.Amount);
            if (availableFunds < command.Amount)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Requested amount {command.Amount} exceeds the limit.");
            }

            var transactions = TransactionCalculator.Create(budgets, command.Amount);
            request.Title = command.Title;
            request.Amount = command.Amount;
            request.Date = command.Date.ToLocalTime();
            await _requestRepository.AddOrUpdateTransactions(request.Id, transactions);

            await _unitOfWork.SaveChanges(cancellationToken);

            return true;
        }
    }
}
