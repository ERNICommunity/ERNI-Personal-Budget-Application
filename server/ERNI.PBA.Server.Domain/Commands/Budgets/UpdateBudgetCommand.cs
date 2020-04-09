namespace ERNI.PBA.Server.Domain.Commands.Budgets
{
    public class UpdateBudgetCommand : CommandBase<bool>
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }
    }
}
