using System;
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
using ERNI.PBA.Server.Domain.Models.Responses;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Business.Queries.Budgets
{
    public class GetTeamBudgetByYearQuery : Query<int, BudgetOutputModel[]>, IGetTeamBudgetByYearQuery
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;

        public GetTeamBudgetByYearQuery(
            IUserRepository userRepository,
            IBudgetRepository budgetRepository)
        {
            _userRepository = userRepository;
            _budgetRepository = budgetRepository;
        }

        protected override async Task<BudgetOutputModel[]> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(principal.GetId(), cancellationToken);

            var budgets = await _budgetRepository.GetTeamBudgets(principal.GetId(), parameter, cancellationToken);
            if (!budgets.Any())
            {
                return Array.Empty<BudgetOutputModel>();
            }

            var masterBudget = budgets.SingleOrDefault(x => x.UserId == user.Id);
            if (masterBudget == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Cumulative budget does not exists");
            }

            var amount = budgets.Sum(_ => _.Amount);
            var amountLeft = amount - budgets.SelectMany(_ => _.Transactions.Where(x => x.Request.State != RequestState.Rejected)).Sum(_ => _.Amount);

            var model = new BudgetOutputModel
            {
                Id = masterBudget.Id,
                Year = masterBudget.Year,
                Amount = amount,
                AmountLeft = amountLeft,
                Title = masterBudget.Title,
                Type = masterBudget.BudgetType,
                Requests = masterBudget.Requests.Select(_ => new RequestOutputModel
                {
                    Id = _.Id,
                    Title = _.Title,
                    Amount = _.Amount,
                    Date = _.Date,
                    CreateDate = _.CreateDate,
                    State = _.State
                })
            };

            return new[] { model };
        }
    }
}
