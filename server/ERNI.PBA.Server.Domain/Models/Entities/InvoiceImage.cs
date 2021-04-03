namespace ERNI.PBA.Server.Domain.Models.Entities
{
    public class InvoiceImage
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int RequestId { get; set; }

        public Request Request { get; set; } = null!;

        public byte[] Data { get; set; } = null!;
    }
}