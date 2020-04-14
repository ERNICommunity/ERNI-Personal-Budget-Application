using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Models.Outputs.InvoiceImages;

namespace ERNI.PBA.Server.Domain.Interfaces.Queries.InvoiceImages
{
    public interface IGetInvoiceImageFileQuery : IQuery<int, ImageFileModel>
    {
    }
}
