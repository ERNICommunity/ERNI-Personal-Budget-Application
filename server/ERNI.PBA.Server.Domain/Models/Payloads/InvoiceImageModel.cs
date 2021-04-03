using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Domain.Models.Payloads
{
    public class InvoiceImageModel
    {
        public int RequestId { get; set; }

        public IFormFile File { get; set; } = null!;
    }
}