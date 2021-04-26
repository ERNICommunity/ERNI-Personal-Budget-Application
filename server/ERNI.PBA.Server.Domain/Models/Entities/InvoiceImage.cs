namespace ERNI.PBA.Server.Domain.Models.Entities
{
    public class InvoiceImage
    {
        public int Id { get; set; }

        public int RequestId { get; set; }

        public Request Request { get; set; } = null!;

        public string Filename { get; set; } = null!;

        public string MimeType { get; set; } = null!;

        public string BlobPath { get; set; } = null!;
    }
}