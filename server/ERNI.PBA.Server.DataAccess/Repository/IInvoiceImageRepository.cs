using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface IInvoiceImageRepository
    {
        Task<Tuple<int, string>[]> GetInvoiceImagesNameId(int requestId, CancellationToken cancellationToken);
        Task AddInvoiceImage(InvoiceImage invoiceImage, CancellationToken cancellationToken);
        Task<InvoiceImage> GetInvoiceImage(int imageId, CancellationToken cancellationToken);
    }
}
