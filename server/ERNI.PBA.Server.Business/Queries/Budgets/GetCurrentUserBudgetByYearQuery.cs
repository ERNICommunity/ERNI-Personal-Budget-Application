using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Responses;

namespace ERNI.PBA.Server.Business.Queries.Budgets
{
    public class GetCurrentUserBudgetByYearQuery : Query<int, IEnumerable<BudgetOutputModel>>, IGetCurrentUserBudgetByYearQuery
    {
        private readonly IBudgetRepository _budgetRepository;

        public GetCurrentUserBudgetByYearQuery(IBudgetRepository budgetRepository) => _budgetRepository = budgetRepository;

        protected override async Task<IEnumerable<BudgetOutputModel>> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetSingleBudgets(principal.GetId(), parameter, cancellationToken);

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
