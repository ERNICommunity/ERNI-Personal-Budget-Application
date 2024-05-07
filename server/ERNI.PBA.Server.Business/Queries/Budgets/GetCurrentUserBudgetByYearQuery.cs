using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Responses;
using ERNI.PBA.Server.Domain.Security;

namespace ERNI.PBA.Server.Business.Queries.Budgets
{
    public class GetCurrentUserBudgetByYearQuery : Query<int, IEnumerable<BudgetOutputModel>>, IGetCurrentUserBudgetByYearQuery
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUserRepository _userRepository;

        public GetCurrentUserBudgetByYearQuery(IBudgetRepository budgetRepository, IUserRepository userRepository)
        {
            _budgetRepository = budgetRepository;
            _userRepository = userRepository;
        }

        protected override async Task<IEnumerable<BudgetOutputModel>> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(principal.GetId(), cancellationToken)
                       ?? throw AppExceptions.AuthorizationException();

            IEnumerable<Budget> budgets;

            if (principal.IsInRole(Roles.CommunityLeader))
            {
                budgets = await _budgetRepository.GetBudgets(user.Id, parameter, new[] { BudgetTypeEnum.TeamBudget, BudgetTypeEnum.CommunityBudget }, cancellationToken);

                budgets = budgets.Concat(await _budgetRepository.GetBudgets(parameter, BudgetTypeEnum.CommunityBudget,
                    cancellationToken));
            }
            else
            {
                budgets = await _budgetRepository.GetBudgets(user.Id, parameter, cancellationToken);
            }

            return budgets.Select(budget => new BudgetOutputModel
            {
                Id = budget.Id,
                Year = budget.Year,
                Amount = budget.Amount,
                AmountLeft = budget.Amount - budget.Transactions
                                 .Where(t => t.Request.State != RequestState.Rejected)
                                 .Sum(_ => _.Amount),
                Title = budget.Title,
                Type = budget.BudgetType,
                IsEditable = budget.UserId == user.Id && budget.Year == DateTime.Now.Year,
                Requests = budget.Transactions.Select(_ => new RequestOutputModel
                {
                    Id = _.Request.Id,
                    Title = _.Request.Title,
                    Amount = _.Amount,
                    CreateDate = _.Request.CreateDate,
                    State = _.Request.State
                }).OrderByDescending(r => r.CreateDate)
            });
        }
    }
}
