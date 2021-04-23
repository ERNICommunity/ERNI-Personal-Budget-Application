using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Projection;
using Microsoft.EntityFrameworkCore;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class InvoiceImageRepository : IInvoiceImageRepository
    {
        private readonly DatabaseContext _context;

        public InvoiceImageRepository(DatabaseContext context) => _context = context;

        public Task<InvoiceImageProjection[]> GetInvoiceImages(int requestId, CancellationToken cancellationToken) =>
            _context.InvoiceImages
                .Where(image => image.RequestId == requestId)
                .Select(image => new InvoiceImageProjection { Id = image.Id, Name = image.Name })
                .ToArrayAsync(cancellationToken: cancellationToken);

        public async Task AddInvoiceImage(InvoiceImage invoiceImage, CancellationToken cancellationToken) =>
            await _context.InvoiceImages.AddAsync(invoiceImage, cancellationToken);

        public async Task<InvoiceImage> GetInvoiceImage(int imageId, CancellationToken cancellationToken) =>
            await _context.InvoiceImages.FirstOrDefaultAsync(image => image.Id == imageId, cancellationToken);

        public async Task<(int requestId, int invoiceCount)[]> GetInvoiceCounts(int[] requestIds,
            CancellationToken cancellationToken)
        {
            var result = await _context.InvoiceImages.Where(_ => requestIds.Contains(_.RequestId))
                .GroupBy(_ => _.RequestId)
                .Select(_ => new {requestId = _.Key, count = _.Count()}).ToArrayAsync(cancellationToken);

            return result.Select(_ => (Key: _.requestId, _.count)).ToArray();
        }
    }
}