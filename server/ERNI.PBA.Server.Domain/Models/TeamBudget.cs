namespace ERNI.PBA.Server.Domain.Models
{
    public class TeamBudget
    {
        public int BudgetId { get; set; }

        public int UserId { get; set; }

        public decimal Amount { get; set; }
    }
}