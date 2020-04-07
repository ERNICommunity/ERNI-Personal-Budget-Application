using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Entities;
using ERNI.PBA.Server.Domain.Entities.Projection;

namespace ERNI.PBA.Server.Domain.Interfaces.Repositories
{
    public interface IInvoiceImageRepository
    {
        Task<InvoiceImageProjection[]> GetInvoiceImages(int requestId, CancellationToken cancellationToken);
        Task AddInvoiceImage(InvoiceImage invoiceImage, CancellationToken cancellationToken);
        Task<InvoiceImage> GetInvoiceImage(int imageId, CancellationToken cancellationToken);
    }
}
