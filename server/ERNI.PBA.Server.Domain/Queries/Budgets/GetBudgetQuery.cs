using System.Security.Claims;
using ERNI.PBA.Server.Domain.Models.Outputs.Budgets;

namespace ERNI.PBA.Server.Domain.Queries.Budgets
{
    public class GetBudgetQuery : QueryBase<SingleBudgetOutputModel>
    {
        public ClaimsPrincipal Principal { get; set; }

        public int BudgetId { get; set; }
    }
}
