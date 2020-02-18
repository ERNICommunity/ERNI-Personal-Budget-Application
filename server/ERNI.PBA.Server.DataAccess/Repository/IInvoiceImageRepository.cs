using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface IInvoiceImageRepository
    {
        Task<string[]> GetInvoiceImagesName(int requestId, CancellationToken cancellationToken);
        Task AddInvoiceImage(InvoiceImage invoiceImage, CancellationToken cancellationToken);
    }
}
