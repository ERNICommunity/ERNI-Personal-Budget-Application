using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class InvoiceImageRepository : IInvoiceImageRepository
    {
        private readonly DatabaseContext _context;

        public InvoiceImageRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Tuple<int,string>[]> GetInvoiceImagesNameId(int requestId, CancellationToken cancellationToken)
        {
            return await _context.InvoiceImages
                .Where(image => image.RequestId == requestId)
                .Select(image => new Tuple<int, string>(image.Id, image.Name + image.Extension))
                .ToArrayAsync(cancellationToken);
        }

        public async Task AddInvoiceImage(InvoiceImage invoiceImage, CancellationToken cancellationToken)
        {
            await _context.InvoiceImages.AddAsync(invoiceImage, cancellationToken);
        }

        public async Task<InvoiceImage> GetInvoiceImage(int imageId, CancellationToken cancellationToken)
        {
            return await _context.InvoiceImages.FirstAsync(image => image.Id == imageId, cancellationToken);
        }
    }
}
