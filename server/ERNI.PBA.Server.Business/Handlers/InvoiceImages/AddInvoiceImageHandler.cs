using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain;
using ERNI.PBA.Server.Domain.Commands.InvoiceImages;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Business.Handlers.InvoiceImages
{
#pragma warning disable CS0162 // Unreachable code detected
    public class AddInvoiceImageHandler : IRequestHandler<AddInvoiceImageCommand, bool>
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IInvoiceImageRepository _invoiceImageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddInvoiceImageHandler(
            IRequestRepository requestRepository,
            IInvoiceImageRepository invoiceImageRepository,
            IUnitOfWork unitOfWork)
        {
            _requestRepository = requestRepository;
            _invoiceImageRepository = invoiceImageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(AddInvoiceImageCommand command, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();

            var requestId = command.RequestId;
            var request = await _requestRepository.GetRequest(requestId, cancellationToken);
            if (!command.Principal.IsInRole(Roles.Admin) && command.Principal.GetId() != request.UserId)
            {
                throw AppExceptions.AuthorizationException();
            }

            byte[] buffer;
            if (command.File == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest);
            }

            var fullName = command.File.FileName;
            if (command.File.Length > 1048576)
            {
                throw new OperationErrorException(StatusCodes.Status413PayloadTooLarge);
            }

            using (var openReadStream = command.File.OpenReadStream())
            {
                buffer = new byte[command.File.Length];
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

            return true;
        }
    }
#pragma warning restore CS0162 // Unreachable code detected
}
