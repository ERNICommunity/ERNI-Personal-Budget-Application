using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Model.PendingRequests
{
    public class InvoiceImageModel
    {
        public string RequestId { get; set; }
        public IFormFile FileKey { get; set; }
    }
}
