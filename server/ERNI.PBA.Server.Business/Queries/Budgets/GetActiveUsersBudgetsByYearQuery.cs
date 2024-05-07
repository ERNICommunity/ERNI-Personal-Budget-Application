using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Responses;

namespace ERNI.PBA.Server.Business.Queries.Budgets
{
    public class GetActiveUsersBudgetsByYearQuery(IBudgetRepository budgetRepository) : Query<int, IEnumerable<BudgetOutputModel>>, IGetActiveUsersBudgetsByYearQuery
    {
        protected override async Task<IEnumerable<BudgetOutputModel>> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var budgets = await budgetRepository.GetBudgetsByYear(parameter, cancellationToken);

            var amounts = (await budgetRepository.GetTotalAmountsByYear(parameter, cancellationToken))
                .ToDictionary(_ => _.BudgetId, _ => _.Amount);

            return budgets.Select(b =>
                    new BudgetOutputModel
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Year = b.Year,
                        Amount = b.Amount,
                        AmountLeft = b.Amount - amounts[b.Id],
                        Type = b.BudgetType,
                        User = new UserOutputModel
                        {
                            Id = b.User.Id,
                            FirstName = b.User.FirstName,
                            LastName = b.User.LastName,
                        }
                    })
                .OrderBy(_ => _.User.LastName).ThenBy(_ => _.User.FirstName);
        }
    }
}
