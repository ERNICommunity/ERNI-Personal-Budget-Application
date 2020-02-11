using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface IInvoiceImageRepository
    {
        Task AddInvoiceImage(InvoiceImage invoiceImage, CancellationToken cancellationToken);
    }
}
