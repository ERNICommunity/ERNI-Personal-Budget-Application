namespace ERNI.PBA.Server.Domain.Models.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        public int RequestId { get; set; }

        public Request Request { get; set; } = null!;

        public int BudgetId { get; set; }

        public Budget Budget { get; set; } = null!;

        public decimal Amount { get; set; }
    }
}