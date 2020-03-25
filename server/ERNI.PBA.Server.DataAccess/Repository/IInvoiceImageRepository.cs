using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Model.Projection;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface IInvoiceImageRepository
    {
        Task<InvoiceImageProjection[]> GetInvoiceImages(int requestId, CancellationToken cancellationToken);
        Task AddInvoiceImage(InvoiceImage invoiceImage, CancellationToken cancellationToken);
        Task<InvoiceImage> GetInvoiceImage(int imageId, CancellationToken cancellationToken);
    }
}
