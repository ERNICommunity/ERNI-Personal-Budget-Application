using System.Collections.Generic;
using System.Security.Claims;
using ERNI.PBA.Server.Domain.Output.InvoiceImages;

namespace ERNI.PBA.Server.Domain.Queries.InvoiceImages
{
    public class GetInvoiceImagesQuery : QueryBase<IEnumerable<ImageOutputModel>>
    {
        public ClaimsPrincipal Principal { get; set; }

        public int RequestId { get; set; }
    }
}
