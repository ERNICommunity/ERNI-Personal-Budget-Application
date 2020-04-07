using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Model.PendingRequests;
using ERNI.PBA.Server.Host.Queries.Budgets;
using MediatR;

namespace ERNI.PBA.Server.Host.Handlers.Budgets
{
    public class GetActiveUsersBudgetsByYearHandler : IRequestHandler<GetActiveUsersBudgetsByYearQuery, IEnumerable<BudgetOutputModel>>
    {
        private readonly IBudgetRepository _budgetRepository;

        public GetActiveUsersBudgetsByYearHandler(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public async Task<IEnumerable<BudgetOutputModel>> Handle(GetActiveUsersBudgetsByYearQuery request, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetBudgetsByYear(request.Year, cancellationToken);

            var amounts = (await _budgetRepository.GetTotalAmountsByYear(request.Year, cancellationToken))
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
