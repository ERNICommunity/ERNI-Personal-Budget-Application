using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Commands.Budgets
{
    public class CreateBudgetCommand : CommandBase<bool>
    {
        public int UserId { get; set; }

        public string Title { get; set; }

        public int CurrentYear { get; set; }

        public decimal Amount { get; set; }

        public BudgetTypeEnum BudgetType { get; set; }
    }
}
