using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.InvoiceImages;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Payloads;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.Business.Commands.InvoiceImages
{
    public class AddInvoiceImageCommand : Command<InvoiceImageModel>, IAddInvoiceImageCommand
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IInvoiceImageRepository _invoiceImageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddInvoiceImageCommand(
            IRequestRepository requestRepository,
            IUserRepository userRepository,
            IInvoiceImageRepository invoiceImageRepository,
            IUnitOfWork unitOfWork)
        {
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            _invoiceImageRepository = invoiceImageRepository;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Execute(InvoiceImageModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var requestId = parameter.RequestId;
            var request = await _requestRepository.GetRequest(requestId, cancellationToken);
            var user = await _userRepository.GetUser(principal.GetId(), cancellationToken);
            if (!principal.IsInRole(Roles.Admin) && user.Id != request.UserId)
            {
                throw AppExceptions.AuthorizationException();
            }

            byte[] buffer;
            if (parameter.File == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest);
            }

            var fullName = parameter.File.FileName;
            if (parameter.File.Length > 1048576)
            {
                throw new OperationErrorException(StatusCodes.Status413PayloadTooLarge);
            }

            using (var openReadStream = parameter.File.OpenReadStream())
            {
                buffer = new byte[parameter.File.Length];
                openReadStream.Read(buffer, 0, buffer.Length);
            }

            var image = new InvoiceImage
            {
                Data = buffer,
                Name = fullName,
                RequestId = requestId
            };

            await _invoiceImageRepository.AddInvoiceImage(image, cancellationToken);

            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}