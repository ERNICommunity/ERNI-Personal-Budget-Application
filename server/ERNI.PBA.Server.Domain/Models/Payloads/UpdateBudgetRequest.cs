namespace ERNI.PBA.Server.Domain.Models.Payloads
{
    public class UpdateBudgetRequest
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }
    }
}