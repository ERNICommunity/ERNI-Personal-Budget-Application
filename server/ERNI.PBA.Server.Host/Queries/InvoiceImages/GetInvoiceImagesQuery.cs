using System.Collections.Generic;
using System.Security.Claims;
using ERNI.PBA.Server.Host.Model.InvoiceImages;

namespace ERNI.PBA.Server.Host.Queries.InvoiceImages
{
    public class GetInvoiceImagesQuery : QueryBase<IEnumerable<ImageOutputModel>>
    {
        public ClaimsPrincipal Principal { get; set; }

        public int RequestId { get; set; }
    }
}
