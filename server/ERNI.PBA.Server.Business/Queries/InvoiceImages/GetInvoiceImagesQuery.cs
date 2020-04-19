using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Queries.InvoiceImages;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Responses.InvoiceImages;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Business.Queries.InvoiceImages
{
#pragma warning disable CS0162 // Unreachable code detected
    public class GetInvoiceImagesQuery : Query<int, IEnumerable<ImageOutputModel>>, IGetInvoiceImagesQuery
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IInvoiceImageRepository _invoiceImageRepository;

        public GetInvoiceImagesQuery(
            IRequestRepository requestRepository,
            IInvoiceImageRepository invoiceImageRepository)
        {
            _requestRepository = requestRepository;
            _invoiceImageRepository = invoiceImageRepository;
        }

        protected override async Task<IEnumerable<ImageOutputModel>> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();

            var request = await _requestRepository.GetRequest(parameter, cancellationToken);
            if (request == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Not a valid id");
            }

            if (!principal.IsInRole(Roles.Admin) && principal.GetId() != request.UserId)
            {
                throw AppExceptions.AuthorizationException();
            }

            var imagesName = await _invoiceImageRepository.GetInvoiceImages(parameter, cancellationToken);
            return imagesName.Select(image => new ImageOutputModel
            {
                Id = image.Id,
                Name = image.Name
            });
        }
    }
#pragma warning restore CS0162 // Unreachable code detected
}
