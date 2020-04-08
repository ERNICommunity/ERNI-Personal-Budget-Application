using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Outputs.InvoiceImages;
using ERNI.PBA.Server.Domain.Queries.InvoiceImages;
using ERNI.PBA.Server.Domain.Security;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace ERNI.PBA.Server.Business.Handlers.InvoiceImages
{
#pragma warning disable CS0162 // Unreachable code detected
    public class GetInvoiceImageFileHandler : IRequestHandler<GetInvoiceImageFileQuery, ImageFileModel>
    {
        private readonly IInvoiceImageRepository _invoiceImageRepository;
        private readonly IRequestRepository _requestRepository;

        public GetInvoiceImageFileHandler(
            IInvoiceImageRepository invoiceImageRepository,
            IRequestRepository requestRepository)
        {
            _invoiceImageRepository = invoiceImageRepository;
            _requestRepository = requestRepository;
        }

        public async Task<ImageFileModel> Handle(GetInvoiceImageFileQuery query, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();

            var image = await _invoiceImageRepository.GetInvoiceImage(query.ImageId, cancellationToken);
            if (image == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Not a valid id");
            }

            var request = await _requestRepository.GetRequest(image.RequestId, cancellationToken);
            if (request == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Not a valid id");
            }

            if (!query.Principal.IsInRole(Roles.Admin) && query.Principal.GetId() != request.UserId)
            {
                throw AppExceptions.AuthorizationException();
            }

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(image.Name, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return new ImageFileModel
            {
                InvoiceImage = image,
                ContentType = contentType
            };
        }
    }
#pragma warning restore CS0162 // Unreachable code detected
}
