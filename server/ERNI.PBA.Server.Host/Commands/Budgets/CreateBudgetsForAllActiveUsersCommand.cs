using ERNI.PBA.Server.Domain.Models;

namespace ERNI.PBA.Server.Host.Commands.Budgets
{
    public class CreateBudgetsForAllActiveUsersCommand : CommandBase<bool>
    {
        public string Title { get; set; }

        public int CurrentYear { get; set; }

        public decimal Amount { get; set; }

        public BudgetTypeEnum BudgetType { get; set; }
    }
}
