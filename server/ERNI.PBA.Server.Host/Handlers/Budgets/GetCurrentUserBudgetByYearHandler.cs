using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Model.PendingRequests;
using ERNI.PBA.Server.Host.Queries.Budgets;
using MediatR;

namespace ERNI.PBA.Server.Host.Handlers.Budgets
{
    public class GetCurrentUserBudgetByYearHandler : IRequestHandler<GetCurrentUserBudgetByYearQuery, IEnumerable<BudgetOutputModel>>
    {
        private readonly IBudgetRepository _budgetRepository;

        public GetCurrentUserBudgetByYearHandler(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public async Task<IEnumerable<BudgetOutputModel>> Handle(GetCurrentUserBudgetByYearQuery request, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetSingleBudgets(request.UserId, request.Year, cancellationToken);

            return budgets.Select(budget => new BudgetOutputModel
            {
                Id = budget.Id,
                Year = budget.Year,
                Amount = budget.Amount,
                AmountLeft = budget.Amount - budget.Requests
                                 .Where(_ => _.State != RequestState.Rejected)
                                 .Sum(_ => _.Amount),
                Title = budget.Title,
                Type = budget.BudgetType,
                Requests = budget.Requests.Select(_ => new RequestOutputModel
                {
                    Id = _.Id,
                    Title = _.Title,
                    Amount = _.Amount,
                    Date = _.Date,
                    CreateDate = _.CreateDate,
                    State = _.State
                })
            });
        }
    }
}
