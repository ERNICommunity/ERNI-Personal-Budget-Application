namespace ERNI.PBA.Server.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        public int RequestId { get; set; }

        public int BudgetId { get; set; }

        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public Request Request { get; set; }

        public Budget Budget { get; set; }

        public User User { get; set; }
    }
}
