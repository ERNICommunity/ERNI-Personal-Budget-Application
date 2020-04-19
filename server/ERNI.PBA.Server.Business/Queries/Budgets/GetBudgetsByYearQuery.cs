using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Responses.Budgets;

namespace ERNI.PBA.Server.Business.Queries.Budgets
{
    public class GetBudgetsByYearQuery : Query<int, IEnumerable<SingleBudgetOutputModel>>, IGetBudgetsByYearQuery
    {
        private readonly IBudgetRepository _budgetRepository;

        public GetBudgetsByYearQuery(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        protected override async Task<IEnumerable<SingleBudgetOutputModel>> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetBudgetsByYear(parameter, cancellationToken);

            return budgets.Select(_ => new SingleBudgetOutputModel
            {
                Id = _.Id,
                Title = _.Title,
                Year = _.Year,
                Amount = _.Amount,
                User = new User
                {
                    FirstName = _.User.FirstName,
                    LastName = _.User.LastName
                },
                Type = _.BudgetType
            }).OrderBy(_ => _.User.LastName).ThenBy(_ => _.User.FirstName);
        }
    }
}
