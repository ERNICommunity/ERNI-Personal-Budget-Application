using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Queries.InvoiceImages;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Responses.InvoiceImages;
using ERNI.PBA.Server.Domain.Security;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.Business.Queries.InvoiceImages
{
    public class GetInvoiceImagesQuery : Query<int, IEnumerable<ImageOutputModel>>, IGetInvoiceImagesQuery
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IInvoiceImageRepository _invoiceImageRepository;

        public GetInvoiceImagesQuery(
            IUserRepository userRepository,
            IRequestRepository requestRepository,
            IInvoiceImageRepository invoiceImageRepository)
        {
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _invoiceImageRepository = invoiceImageRepository;
        }

        protected override async Task<IEnumerable<ImageOutputModel>> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(parameter, cancellationToken);
            if (request == null)
            {
                throw new OperationErrorException(ErrorCodes.RequestNotFound, "Not a valid id");
            }

            var user = await _userRepository.GetUser(principal.GetId(), cancellationToken)
                       ?? throw AppExceptions.AuthorizationException();

            if (!principal.IsInRole(Roles.Admin) && !principal.IsInRole(Roles.Finance) && user.Id != request.UserId)
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
}