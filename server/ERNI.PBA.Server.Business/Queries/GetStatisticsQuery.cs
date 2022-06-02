using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.API;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Queries.Requests;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Business.Queries
{
    public class GetStatisticsQuery : Query<int, GetStatisticsQuery.StatisticsModel>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly ILogger _logger;

        public GetStatisticsQuery(
            IBudgetRepository budgetRepository,
            ILogger<GetRequestQuery> logger)
        {
            _budgetRepository = budgetRepository;
            _logger = logger;
        }

        protected override async Task<StatisticsModel> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var stats = await _budgetRepository.GetBudgetStats(parameter);

            return new StatisticsModel
            {
                Budgets = stats.Select(_ => new BudgetStatisticsModel
                {
                    BudgetType = _.type,
                    BudgetCount = _.count,
                    TotalAmount = _.total,
                    TotalSpentAmount = _.totalSpent,
                }).ToArray(),
            };
        }

        public class BudgetStatisticsModel
        {
            public BudgetTypeEnum BudgetType { get; init; }

            public int BudgetCount { get; init; }

            public decimal TotalAmount { get; init; }

            public decimal TotalSpentAmount { get; init; }
        }

        public class StatisticsModel
        {
            public BudgetStatisticsModel[] Budgets { get; init; } = null!;
        }
    }
}
