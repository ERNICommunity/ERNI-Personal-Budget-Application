using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;

namespace ERNI.PBA.Server.Business.Queries;

public class GetStatisticsQuery(
    IBudgetRepository budgetRepository) : Query<int, GetStatisticsQuery.StatisticsModel>
{
    protected override async Task<StatisticsModel> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        var stats = await budgetRepository.GetBudgetStats(parameter);

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
