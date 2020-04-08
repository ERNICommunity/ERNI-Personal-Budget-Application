using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Output.Budgets;
using ERNI.PBA.Server.Domain.Queries.Budgets;
using MediatR;

namespace ERNI.PBA.Server.Business.Handlers.Budgets
{
    public class GetBudgetsByYearHandler : IRequestHandler<GetBudgetsByYearQuery, IEnumerable<SingleBudgetOutputModel>>
    {
        private readonly IBudgetRepository _budgetRepository;

        public GetBudgetsByYearHandler(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public async Task<IEnumerable<SingleBudgetOutputModel>> Handle(GetBudgetsByYearQuery request, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetBudgetsByYear(request.Year, cancellationToken);

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
