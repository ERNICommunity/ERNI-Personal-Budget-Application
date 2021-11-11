using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Projection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class InvoiceImageRepository : IInvoiceImageRepository
    {
        private readonly DatabaseContext _context;

        private readonly BlobContainerClient _blobContainerClient;

        public InvoiceImageRepository(DatabaseContext context, BlobServiceClient blobClient,
            IOptions<BlobStorageSettings> settings) => (_context, _blobContainerClient) = (context,
            blobClient.GetBlobContainerClient(settings.Value.AttachmentDataContainerName));

        public Task<InvoiceImageProjection[]> GetInvoiceImages(int requestId, CancellationToken cancellationToken) =>
            _context.InvoiceImages
                .Where(image => image.RequestId == requestId)
                .Select(image => new InvoiceImageProjection { Id = image.Id, Name = image.Filename })
                .ToArrayAsync(cancellationToken);

        public async Task AddInvoiceImage(InvoiceImage invoiceImage, CancellationToken cancellationToken) =>
            await _context.InvoiceImages.AddAsync(invoiceImage, cancellationToken);

        public async Task<InvoiceImage?> GetInvoiceImage(int imageId, CancellationToken cancellationToken) =>
            await _context.InvoiceImages.FirstOrDefaultAsync(image => image.Id == imageId, cancellationToken);

        public async Task<(int requestId, int invoiceCount)[]> GetInvoiceCounts(int[] requestIds,
            CancellationToken cancellationToken)
        {
            var result = await _context.InvoiceImages.Where(_ => requestIds.Contains(_.RequestId))
                .GroupBy(_ => _.RequestId)
                .Select(_ => new { requestId = _.Key, count = _.Count() }).ToArrayAsync(cancellationToken);

            return result.Select(_ => (Key: _.requestId, _.count)).ToArray();
        }

        public async Task<string> UploadImageDataBlob(byte[] data, int imageId,
            CancellationToken cancellationToken)
        {
            var blobName = $"{imageId}/{Guid.NewGuid():D}";

            await using var ms = new MemoryStream(data);
            await _blobContainerClient.UploadBlobAsync(blobName, ms, cancellationToken);

            return blobName;
        }

        public async Task<byte[]?> DownloadImageDataBlob(string blobName, CancellationToken cancellationToken)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                return null;
            }

            await using var ms = new MemoryStream();
            await blobClient.DownloadToAsync(ms, cancellationToken);

            return ms.ToArray();
        }
    }
}