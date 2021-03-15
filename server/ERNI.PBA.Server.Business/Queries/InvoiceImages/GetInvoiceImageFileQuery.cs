using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Queries.InvoiceImages;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Responses.InvoiceImages;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.Business.Queries.InvoiceImages
{
    public class GetInvoiceImageFileQuery : Query<int, ImageFileModel>, IGetInvoiceImageFileQuery
    {
        private readonly IUserRepository _userRepository;
        private readonly IInvoiceImageRepository _invoiceImageRepository;
        private readonly IRequestRepository _requestRepository;

        public GetInvoiceImageFileQuery(
            IUserRepository userRepository,
            IInvoiceImageRepository invoiceImageRepository,
            IRequestRepository requestRepository)
        {
            _userRepository = userRepository;
            _invoiceImageRepository = invoiceImageRepository;
            _requestRepository = requestRepository;
        }

        protected override async Task<ImageFileModel> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
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

            var user = await _userRepository.GetUser(principal.GetId(), cancellationToken);

            if (!principal.IsInRole(Roles.Admin) && !principal.IsInRole(Roles.Finance) && user.Id != request.UserId)
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
}