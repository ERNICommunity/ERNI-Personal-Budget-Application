using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Models.Responses.InvoiceImages;

namespace ERNI.PBA.Server.Domain.Interfaces.Queries.InvoiceImages
{
    public interface IGetInvoiceImagesQuery : IQuery<int, IEnumerable<ImageOutputModel>>
    {
    }
}
