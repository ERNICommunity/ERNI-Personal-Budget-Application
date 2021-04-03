using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Payloads
{
    public class CreateBudgetRequest
    {
        public int UserId { get; set; }

        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public BudgetTypeEnum BudgetType { get; set; }
    }
}