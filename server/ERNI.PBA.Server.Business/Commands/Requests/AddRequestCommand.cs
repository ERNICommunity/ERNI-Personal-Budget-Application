using System;
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
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Business.Commands.Requests
{
    public class AddRequestCommand : Command<PostRequestModel>, IAddRequestCommand
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddRequestCommand(
            IBudgetRepository budgetRepository,
            IRequestRepository requestRepository,
            IUnitOfWork unitOfWork)
        {
            _budgetRepository = budgetRepository;
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Execute(PostRequestModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            
            if (parameter.Amount < 1)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Amount have to be positive");
            }

            var budget = await _budgetRepository.GetBudget(parameter.BudgetId, cancellationToken);
            var currentYear = DateTime.Now.Year;

            if (budget == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Budget {parameter.BudgetId} was not found.");
            }

            if (budget.BudgetType == BudgetTypeEnum.TeamBudget)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "No Access for request!");
            }

            var requestedAmount = await _budgetRepository.GetTotalRequestedAmount(parameter.BudgetId, cancellationToken);

            if (parameter.Amount > budget.Amount - requestedAmount)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Requested amount {parameter.Amount} exceeds the limit.");
            }

            var userId = principal.GetId();
            var request = new Request
            {
                BudgetId = budget.Id,
                UserId = userId,
                Year = currentYear,
                Title = parameter.Title,
                Amount = parameter.Amount,
                Date = parameter.Date.ToLocalTime(),
                CreateDate = DateTime.Now,
                State = RequestState.Pending,
                Transactions = new[]
                {
                    new Transaction
                    {
                        BudgetId = budget.Id,
                        UserId = userId,
                        Amount = parameter.Amount
                    }
                }
            };

            await _requestRepository.AddRequest(request);

            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}
