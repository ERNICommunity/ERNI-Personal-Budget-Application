using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class InvoiceImageRepository : IInvoiceImageRepository
    {
        private readonly DatabaseContext _context;

        public InvoiceImageRepository(DatabaseContext context)
        {
            _context = context;
        }
        public async Task AddInvoiceImage(InvoiceImage invoiceImage, CancellationToken cancellationToken)
        {
            await _context.InvoiceImages.AddAsync(invoiceImage, cancellationToken);
        }
    }
}
