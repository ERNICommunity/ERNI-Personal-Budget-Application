using ERNI.PBA.API;

namespace ERNI.PBA.Server.Domain.Models.Payloads
{
    public class CreateBudgetsForAllActiveUsersRequest
    {
        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public BudgetTypeEnum BudgetType { get; set; }
    }
}