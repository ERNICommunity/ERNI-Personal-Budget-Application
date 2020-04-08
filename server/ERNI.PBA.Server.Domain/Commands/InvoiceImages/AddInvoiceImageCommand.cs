using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Domain.Commands.InvoiceImages
{
    public class AddInvoiceImageCommand : CommandBase<bool>
    {
        public ClaimsPrincipal Principal { get; set; }

        public int RequestId { get; set; }

        public IFormFile File { get; set; }
    }
}
