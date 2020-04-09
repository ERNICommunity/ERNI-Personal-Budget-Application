namespace ERNI.PBA.Server.Host.Commands.Budgets
{
    public class TransferBudgetCommand : CommandBase<bool>
    {
        public int UserId { get; set; }

        public int BudgetId { get; set; }
    }
}
