using System.Security.Claims;
using ERNI.PBA.Server.Domain.Output.Budgets;

namespace ERNI.PBA.Server.Domain.Queries.Budgets
{
    public class GetBudgetQuery : QueryBase<SingleBudgetOutputModel>
    {
        public ClaimsPrincipal Principal { get; set; }

        public int BudgetId { get; set; }
    }
}
