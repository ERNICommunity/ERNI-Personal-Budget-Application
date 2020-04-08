using ERNI.PBA.Server.Domain.Models.Outputs;

namespace ERNI.PBA.Server.Domain.Queries.Budgets
{
    public class GetBudgetByYearQuery : QueryBase<BudgetOutputModel[]>
    {
        public int UserId { get; set; }

        public int Year { get; set; }
    }
}
