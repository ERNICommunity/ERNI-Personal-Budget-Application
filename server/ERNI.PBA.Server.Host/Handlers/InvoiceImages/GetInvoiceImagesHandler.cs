using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Exceptions;
using ERNI.PBA.Server.Host.Model.InvoiceImages;
using ERNI.PBA.Server.Host.Queries.InvoiceImages;
using ERNI.PBA.Server.Host.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Handlers.InvoiceImages
{
#pragma warning disable CS0162 // Unreachable code detected
    public class GetInvoiceImagesHandler : IRequestHandler<GetInvoiceImagesQuery, IEnumerable<ImageOutputModel>>
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IInvoiceImageRepository _invoiceImageRepository;

        public GetInvoiceImagesHandler(
            IRequestRepository requestRepository,
            IInvoiceImageRepository invoiceImageRepository)
        {
            _requestRepository = requestRepository;
            _invoiceImageRepository = invoiceImageRepository;
        }

        public async Task<IEnumerable<ImageOutputModel>> Handle(GetInvoiceImagesQuery query, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();

            var request = await _requestRepository.GetRequest(query.RequestId, cancellationToken);
            if (request == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Not a valid id");
            }

            if (!query.Principal.IsInRole(Roles.Admin) && query.Principal.GetId() != request.UserId)
            {
                throw AppExceptions.AuthorizationException();
            }

            var imagesName = await _invoiceImageRepository.GetInvoiceImages(query.RequestId, cancellationToken);
            return imagesName.Select(image => new ImageOutputModel
            {
                Id = image.Id,
                Name = image.Name
            });
        }
    }
#pragma warning restore CS0162 // Unreachable code detected
}
