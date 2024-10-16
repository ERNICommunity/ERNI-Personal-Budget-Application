﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Payloads;

namespace ERNI.PBA.Server.Business.Commands.Budgets
{
    public class CreateBudgetsForAllActiveUsersCommand(
        IUserRepository userRepository,
        IBudgetRepository budgetRepository,
        IUnitOfWork unitOfWork) : Command<CreateBudgetsForAllActiveUsersRequest>, ICreateBudgetsForAllActiveUsersCommand
    {
        protected override async Task Execute(CreateBudgetsForAllActiveUsersRequest parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var currentYear = DateTime.Now.Year;
            IEnumerable<User> users = await userRepository.GetAllUsers(_ => _.State == UserState.Active, cancellationToken);

            var budgetType = BudgetType.Types.Single(_ => _.Id == parameter.BudgetType);

            if (budgetType.SinglePerUser)
            {
                var budgets =
                    (await budgetRepository.GetBudgetsByYear(DateTime.Now.Year, cancellationToken)).Where(_ =>
                        _.BudgetType == budgetType.Id).Select(_ => _.UserId).ToHashSet();
                users = users.Where(_ => !budgets.Contains(_.Id));
            }

            foreach (var user in users)
            {
                var budget = new Budget()
                {
                    UserId = user.Id,
                    Year = currentYear,
                    Amount = parameter.Amount,
                    BudgetType = parameter.BudgetType,
                    Title = parameter.Title
                };

                await budgetRepository.AddBudgetAsync(budget);
            }

            await unitOfWork.SaveChanges(cancellationToken);
        }
    }
}
