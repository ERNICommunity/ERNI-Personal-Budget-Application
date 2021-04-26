using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Payloads;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Business.Commands.Budgets
{
    public class CreateBudgetCommand : Command<CreateBudgetRequest>, ICreateBudgetCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateBudgetCommand(
            IUserRepository userRepository,
            IBudgetRepository budgetRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _budgetRepository = budgetRepository;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Execute(CreateBudgetRequest parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var currentYear = DateTime.Now.Year;
            var userId = parameter.UserId;
            var user = await _userRepository.GetUser(userId, cancellationToken);
            if (user == null || user.State != UserState.Active)
            {
                throw new OperationErrorException(ErrorCodes.UserNotFound, $"No active user with id {userId} found");
            }

            var budgets = await _budgetRepository.GetBudgets(userId, currentYear, cancellationToken);
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

            _budgetRepository.AddBudget(budget);

            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}
