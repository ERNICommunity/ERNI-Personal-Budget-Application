using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Model.PendingRequests
{
    public class InvoiceImageModel
    {
        public int RequestId { get; set; }
        public IFormFile File { get; set; }
    }
}
