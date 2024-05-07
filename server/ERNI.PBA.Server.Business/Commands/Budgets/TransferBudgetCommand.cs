using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Payloads;

namespace ERNI.PBA.Server.Business.Commands.Budgets
{
    public class TransferBudgetCommand(
        IUserRepository userRepository,
        IBudgetRepository budgetRepository,
        IUnitOfWork unitOfWork) : Command<TransferBudgetModel>, ITransferBudgetCommand
    {
        protected override async Task Execute(TransferBudgetModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var budget = await budgetRepository.GetBudget(parameter.BudgetId, cancellationToken);
            if (budget == null)
            {
                throw new OperationErrorException(ErrorCodes.BudgetNotFound, $"Budget with id {parameter.BudgetId} not found");
            }

            if (!BudgetType.Types.Single(type => type.Id == budget.BudgetType).IsTransferable)
            {
                throw new OperationErrorException(ErrorCodes.UnknownError, $"Budget with id {parameter.BudgetId} can not be transferred");
            }

            var user = await userRepository.GetUser(parameter.UserId, cancellationToken);
            if (user == null)
            {
                throw new OperationErrorException(ErrorCodes.UserNotFound, $"User with id {parameter.UserId} not found");
            }

            budget.UserId = parameter.UserId;

            await unitOfWork.SaveChanges(cancellationToken);
        }
    }
}
