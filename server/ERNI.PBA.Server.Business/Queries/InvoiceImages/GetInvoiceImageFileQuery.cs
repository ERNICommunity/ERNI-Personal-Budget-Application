using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain;

namespace ERNI.PBA.Server.Business.Queries.InvoiceImages
{
    public class GetInvoiceImageFileQuery : Query<int, GetInvoiceImageFileQuery.InvoiceModel>
    {
        private readonly IInvoiceImageRepository _invoiceImageRepository;
        private readonly IRequestRepository _requestRepository;

        public GetInvoiceImageFileQuery(
            IInvoiceImageRepository invoiceImageRepository,
            IRequestRepository requestRepository)
        {
            _invoiceImageRepository = invoiceImageRepository;
            _requestRepository = requestRepository;
        }

        protected override async Task<InvoiceModel> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var image = await _invoiceImageRepository.GetInvoiceImage(parameter, cancellationToken)
                ?? throw new OperationErrorException(ErrorCodes.InvalidId, "Not a valid id");

            if (await _requestRepository.GetRequest(image.RequestId, cancellationToken) == null)
            {
                throw new OperationErrorException(ErrorCodes.InvalidId, "Not a valid id");
            }

            // var provider = new FileExtensionContentTypeProvider();
            // if (!provider.TryGetContentType(image.Name, out var contentType))
            // {
            //    contentType = "application/octet-stream";
            // }

            var data = await _invoiceImageRepository
                .DownloadImageDataBlob(image.BlobPath, cancellationToken)
                .NotNullAsync(ErrorCodes.AttachmentDataNotFound, "Unable to load the specified attachment data");

            return new InvoiceModel(image.Id, image.Filename, image.MimeType, data);
        }

        public sealed record InvoiceModel(int Id, string Filename, string MimeType, byte[] Data);

    }
}