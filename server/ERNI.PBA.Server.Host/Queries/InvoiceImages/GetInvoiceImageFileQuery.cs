using System.Security.Claims;
using ERNI.PBA.Server.Host.Model.InvoiceImages;

namespace ERNI.PBA.Server.Host.Queries.InvoiceImages
{
    public class GetInvoiceImageFileQuery : QueryBase<ImageFileModel>
    {
        public ClaimsPrincipal Principal { get; set; }

        public int ImageId { get; set; }
    }
}
