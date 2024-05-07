using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Payloads;

namespace ERNI.PBA.Server.Business.Commands.Budgets
{
    public class CreateBudgetCommand(
        IUserRepository userRepository,
        IBudgetRepository budgetRepository,
        IUnitOfWork unitOfWork) : Command<CreateBudgetRequest>, ICreateBudgetCommand
    {
        protected override async Task Execute(CreateBudgetRequest parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var currentYear = DateTime.Now.Year;
            var userId = parameter.UserId;
            var user = await userRepository.GetUser(userId, cancellationToken);
            if (user == null || user.State != UserState.Active)
            {
                throw new OperationErrorException(ErrorCodes.UserNotFound, $"No active user with id {userId} found");
            }

            var budgets = await budgetRepository.GetBudgets(userId, currentYear, cancellationToken);
            var budgetType = BudgetType.Types.Single(_ => _.Id == parameter.BudgetType);
            if (budgetType.SinglePerUser && budgets.Any(b => b.BudgetType == parameter.BudgetType))
            {
                throw new OperationErrorException(ErrorCodes.UnknownError, $"User {user.LastName} {user.FirstName}  already has a budget of type {budgetType.Name} assigned for this year");
            }

            var budget = new Budget
            {
                UserId = user.Id,
                Year = currentYear,
                Amount = parameter.Amount,
                BudgetType = parameter.BudgetType,
                Title = parameter.Title
            };

            await budgetRepository.AddBudgetAsync(budget);

            await unitOfWork.SaveChanges(cancellationToken);
        }
    }
}
