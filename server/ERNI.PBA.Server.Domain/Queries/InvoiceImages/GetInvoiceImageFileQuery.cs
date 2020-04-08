using System.Security.Claims;
using ERNI.PBA.Server.Domain.Output.InvoiceImages;

namespace ERNI.PBA.Server.Domain.Queries.InvoiceImages
{
    public class GetInvoiceImageFileQuery : QueryBase<ImageFileModel>
    {
        public ClaimsPrincipal Principal { get; set; }

        public int ImageId { get; set; }
    }
}
