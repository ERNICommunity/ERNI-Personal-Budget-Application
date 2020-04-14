using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Queries.InvoiceImages;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Outputs.InvoiceImages;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace ERNI.PBA.Server.Business.Queries.InvoiceImages
{
#pragma warning disable CS0162 // Unreachable code detected
    public class GetInvoiceImageFileQuery : Query<int, ImageFileModel>, IGetInvoiceImageFileQuery
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

        protected override async Task<ImageFileModel> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();

            var image = await _invoiceImageRepository.GetInvoiceImage(parameter, cancellationToken);
            if (image == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Not a valid id");
            }

            var request = await _requestRepository.GetRequest(image.RequestId, cancellationToken);
            if (request == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Not a valid id");
            }

            if (!principal.IsInRole(Roles.Admin) && principal.GetId() != request.UserId)
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
