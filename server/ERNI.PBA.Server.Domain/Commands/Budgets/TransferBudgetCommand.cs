namespace ERNI.PBA.Server.Domain.Commands.Budgets
{
    public class TransferBudgetCommand : CommandBase<bool>
    {
        public int UserId { get; set; }

        public int BudgetId { get; set; }
    }
}
