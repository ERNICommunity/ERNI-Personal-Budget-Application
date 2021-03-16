namespace ERNI.PBA.Server.Domain.Models.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        public int RequestId { get; set; }

        public int BudgetId { get; set; }

        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public Request Request { get; set; } = null!;

        public Budget Budget { get; set; } = null!;

        public User User { get; set; } = null!;
    }
}