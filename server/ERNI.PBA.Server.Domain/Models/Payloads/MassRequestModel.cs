namespace ERNI.PBA.Server.Domain.Models.Payloads
{
    public class MassRequestModel
    {
        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public int[] Employees { get; set; } = null!;
    }
}