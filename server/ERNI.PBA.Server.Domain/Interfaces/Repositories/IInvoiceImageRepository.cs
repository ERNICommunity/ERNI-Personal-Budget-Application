using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Projection;

namespace ERNI.PBA.Server.Domain.Interfaces.Repositories
{
    public interface IInvoiceImageRepository
    {
        Task<InvoiceImageProjection[]> GetInvoiceImages(int requestId, CancellationToken cancellationToken);

        Task AddInvoiceImage(InvoiceImage invoiceImage, CancellationToken cancellationToken);

        Task<InvoiceImage> GetInvoiceImage(int imageId, CancellationToken cancellationToken);

        Task<(int requestId, int invoiceCount)[]> GetInvoiceCounts(int[] requestIds, CancellationToken cancellationToken);
    }
}
