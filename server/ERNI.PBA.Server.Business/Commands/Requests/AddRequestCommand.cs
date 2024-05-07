using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Business.Commands.Requests;

public class AddRequestCommand(
    IBudgetRepository budgetRepository,
    IRequestRepository requestRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : Command<AddRequestCommand.PostRequestModel, int>
{
    protected override async Task<int> Execute(PostRequestModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUser(principal.GetId(), cancellationToken) ?? throw AppExceptions.AuthorizationException();

        if (string.IsNullOrWhiteSpace(parameter.Title))
        {
            throw new OperationErrorException(ErrorCodes.InvalidTitle, $"Title must not be empty");
        }

        if (parameter.Amount < 0)
        {
            throw new OperationErrorException(ErrorCodes.InvalidAmount, $"The amount must be greater than 0");
        }

        var budget = await budgetRepository.GetBudget(parameter.BudgetId, cancellationToken);
        var currentYear = DateTime.Now.Year;

        if (budget == null)
        {
            throw new OperationErrorException(ErrorCodes.BudgetNotFound, $"Budget {parameter.BudgetId} was not found.");
        }

        if (budget.BudgetType == BudgetTypeEnum.TeamBudget)
        {
            throw new OperationErrorException(ErrorCodes.AccessDenied, "No Access for request!");
        }

        var requestedAmount = await budgetRepository.GetTotalRequestedAmount(parameter.BudgetId, cancellationToken);

        if (parameter.Amount > budget.Amount - requestedAmount)
        {
            throw new OperationErrorException(ErrorCodes.InvalidAmount, $"Requested amount {parameter.Amount} exceeds the limit.");
        }

        if (user.Id != budget.UserId)
        {
            throw AppExceptions.AuthorizationException();
        }

        var request = new Request
        {
            UserId = user.Id,
            Year = currentYear,
            Title = parameter.Title.Trim(),
            Amount = parameter.Amount,
            CreateDate = DateTime.Now,
            State = GetRequestState(budget.BudgetType),
            RequestType = budget.BudgetType,
            Transactions = [
                new Transaction
                {
                    BudgetId = budget.Id,
                    Amount = parameter.Amount,
                    RequestType = budget.BudgetType,
                }
            ]
        };

        await requestRepository.AddRequest(request);

        await unitOfWork.SaveChanges(cancellationToken);

        return request.Id;
    }


    private static RequestState GetRequestState(BudgetTypeEnum budgetType) =>
        budgetType switch
        {
            BudgetTypeEnum.CommunityBudget => RequestState.Approved,
            BudgetTypeEnum.PersonalBudget => RequestState.Pending,
            BudgetTypeEnum.RecreationBudget => RequestState.Pending,
            BudgetTypeEnum.TeamBudget => RequestState.Approved,
            _ => throw new InvalidOperationException($"Unexpected budget type: {budgetType}")
        };

    public class PostRequestModel
    {
        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public int BudgetId { get; set; }
    }
}
