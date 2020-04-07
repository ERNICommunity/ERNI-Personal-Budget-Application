using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Entities;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Host.Commands.Requests;
using ERNI.PBA.Server.Host.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Handlers.Requests
{
    public class AddRequestHandler : IRequestHandler<AddRequestCommand, bool>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddRequestHandler(
            IBudgetRepository budgetRepository,
            IRequestRepository requestRepository,
            IUnitOfWork unitOfWork)
        {
            _budgetRepository = budgetRepository;
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(AddRequestCommand command, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(command.BudgetId, cancellationToken);

            if (budget == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Budget {command.BudgetId} was not found.");
            }

            if (budget.BudgetType == BudgetTypeEnum.TeamBudget)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "No Access for request!");
            }

            var requestedAmount = await _budgetRepository.GetTotalRequestedAmount(command.BudgetId, cancellationToken);

            if (command.Amount > budget.Amount - requestedAmount)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Requested amount {command.Amount} exceeds the amount left ({requestedAmount} of {budget.Amount}).");
            }

            var request = new Request
            {
                BudgetId = budget.Id,
                UserId = command.UserId,
                Year = command.CurrentYear,
                Title = command.Title,
                Amount = command.Amount,
                Date = command.Date.ToLocalTime(),
                CreateDate = DateTime.Now,
                State = RequestState.Pending,
                Transactions = new[]
                {
                    new Transaction
                    {
                        BudgetId = budget.Id,
                        UserId = command.UserId,
                        Amount = command.Amount
                    }
                }
            };

            await _requestRepository.AddRequest(request);

            await _unitOfWork.SaveChanges(cancellationToken);

            return true;
        }
    }
}
