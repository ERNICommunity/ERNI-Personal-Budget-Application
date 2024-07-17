using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain;

namespace ERNI.PBA.Server.Business.Queries.InvoiceImages
{
    public class GetInvoiceImageFileQuery(
        IInvoiceImageRepository invoiceImageRepository) : Query<int, GetInvoiceImageFileQuery.InvoiceModel>
    {
        protected override async Task<InvoiceModel> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var image = await invoiceImageRepository.GetInvoiceImage(parameter, cancellationToken) ?? throw new OperationErrorException(ErrorCodes.InvalidId, "Not a valid id");

            // var provider = new FileExtensionContentTypeProvider();
            // if (!provider.TryGetContentType(image.Name, out var contentType))
            // {
            //    contentType = "application/octet-stream";
            // }

            var data = await invoiceImageRepository
                .DownloadImageDataBlob(image.BlobPath, cancellationToken)
                .NotNullAsync(ErrorCodes.AttachmentDataNotFound, "Unable to load the specified attachment data");

            return new InvoiceModel(image.Id, image.Filename, image.MimeType, data);
        }

        public sealed record InvoiceModel(int Id, string Filename, string MimeType, byte[] Data);

    }
}