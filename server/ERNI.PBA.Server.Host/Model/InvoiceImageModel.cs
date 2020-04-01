using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Model
{
    public class InvoiceImageModel
    {
        public int RequestId { get; set; }

        public IFormFile File { get; set; }
    }
}
