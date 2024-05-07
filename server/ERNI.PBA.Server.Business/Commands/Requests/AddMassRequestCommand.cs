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
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Payloads;
using ERNI.PBA.Server.Domain.Security;

namespace ERNI.PBA.Server.Business.Commands.Requests;

public class AddMassRequestCommand(
    IBudgetRepository budgetRepository,
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork) : Command<MassRequestModel>, IAddMassRequestCommand
{
    protected override async Task Execute(MassRequestModel parameter, ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        if (!principal.IsInRole(Roles.Admin))
        {
            throw AppExceptions.AuthorizationException();
        }

        var currentYear = DateTime.Now.Year;

        var budgets = await budgetRepository.GetBudgetsWithRequestedAmounts(
            budget => parameter.Employees.Contains(budget.UserId) && budget.Year == currentYear && budget.BudgetType == BudgetTypeEnum.PersonalBudget,
            cancellationToken);

        var usersWithMultipleBudgets = budgets.GroupBy(_ => _.Budget.UserId).Where(_ => _.Count() > 1)
            .Select(_ => _.Key).ToArray();
        if (usersWithMultipleBudgets.Length != 0)
        {
            throw new OperationErrorException(ErrorCodes.UnknownError,
                $"Users '{string.Join(", ", usersWithMultipleBudgets)}' have multiple budgets of type {BudgetTypeEnum.PersonalBudget} for year {currentYear}");
        }

        var userWithoutBudgets = parameter.Employees.Except(budgets.Select(_ => _.Budget.UserId)).ToArray();
        if (userWithoutBudgets.Length != 0)
        {
            throw new OperationErrorException(ErrorCodes.UnknownError,
                $"Users '{string.Join(", ", userWithoutBudgets)}' have no {BudgetTypeEnum.PersonalBudget} for year {currentYear}");
        }

        var usersWithNotEnoughBudget =
            budgets.Where(budget => parameter.Amount > budget.Budget.Amount - budget.AmountSpent).ToArray();
        if (usersWithNotEnoughBudget.Length != 0)
        {
            throw new OperationErrorException(ErrorCodes.UnknownError,
                $"Users '{string.Join(", ", usersWithNotEnoughBudget.Select(_ => _.Budget.UserId))}' do not have sufficient amount left on their budgets.");
        }

        var requests = budgets.Select(budget =>
            new Request
            {
                UserId = budget.Budget.UserId,
                Year = currentYear,
                Title = parameter.Title,
                Amount = parameter.Amount,
                CreateDate = DateTime.Now,
                State = RequestState.Approved,
                RequestType = budget.Budget.BudgetType,
                Transactions = [
                    new Transaction
                    {
                        Amount = parameter.Amount,
                        BudgetId = budget.Budget.Id,
                        RequestType = budget.Budget.BudgetType,
                    }
                ]
            });

        await requestRepository.AddRequests(requests);

        await unitOfWork.SaveChanges(cancellationToken);
    }
}
