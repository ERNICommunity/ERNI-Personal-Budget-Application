using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Payloads;

namespace ERNI.PBA.Server.Business.Commands.Budgets;

public class UpdateBudgetCommand(
    IBudgetRepository budgetRepository,
    IUnitOfWork unitOfWork) : Command<UpdateBudgetRequest>, IUpdateBudgetCommand
{
    protected override async Task Execute(UpdateBudgetRequest parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        var budget = await budgetRepository.GetBudget(parameter.Id, cancellationToken) ?? throw new OperationErrorException(ErrorCodes.BudgetNotFound, $"Budget with id {parameter.Id} not found");

        budget.Amount = parameter.Amount;

        await unitOfWork.SaveChanges(cancellationToken);
    }
}
