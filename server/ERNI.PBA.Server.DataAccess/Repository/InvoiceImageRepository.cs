﻿using System.Linq;
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

        public Task<IdNamePair[]> GetInvoiceImagesIdNamePairs(int requestId, CancellationToken cancellationToken)
        {
            return _context.InvoiceImages
                .Where(image => image.RequestId == requestId)
                .Select(image => new IdNamePair()
                {
                    Id = image.Id,
                    Name = image.Name
                })
                .ToArrayAsync(cancellationToken: cancellationToken);
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
