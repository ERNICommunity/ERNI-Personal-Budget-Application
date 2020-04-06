using System.Security.Claims;
using ERNI.PBA.Server.Host.Model.Budgets;

namespace ERNI.PBA.Server.Host.Queries.Budgets
{
    public class GetBudgetQuery : QueryBase<SingleBudgetOutputModel>
    {
        public ClaimsPrincipal Principal { get; set; }

        public int BudgetId { get; set; }
    }
}
