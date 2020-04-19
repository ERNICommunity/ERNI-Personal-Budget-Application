using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Payloads
{
    public class CreateBudgetRequest
    {
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public BudgetTypeEnum BudgetType { get; set; }
    }
}