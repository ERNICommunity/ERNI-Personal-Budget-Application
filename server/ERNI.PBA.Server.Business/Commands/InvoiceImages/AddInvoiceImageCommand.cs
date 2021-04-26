using System;
using System.Globalization;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.Business.Commands.InvoiceImages
{
    public class AddInvoiceImageCommand : Command<AddInvoiceImageCommand.InvoiceImageModel>
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

            var blobPath = await _invoiceImageRepository.UploadImageDataBlob(parameter.Data, request.Id, cancellationToken);

            var image = new InvoiceImage
            {
                Filename = parameter.Filename,
                MimeType = parameter.MimeType, 
                BlobPath = blobPath,
                RequestId = requestId
            };

            await _invoiceImageRepository.AddInvoiceImage(image, cancellationToken);

            await _unitOfWork.SaveChanges(cancellationToken);
        }

        public class InvoiceImageModel
        {
            public InvoiceImageModel(int id, string? filename, string? mimeType, byte[]? data)
            {
                if (data is null || data.Length == 0)
                {
                    // throw new ValidationErrorException(new[] { new ValidationError(ValidationErrorCodes.EmptyField, $"{nameof(Data)} cannot be empty.") })
                    throw new InvalidOperationException();
                }

                if (data.Length > 5 * 1024 * 1024)
                {
                    // throw new ValidationErrorException(new[] { new ValidationError(ValidationErrorCodes.OutOfRange, $"{nameof(Data)} cannot exceed 5 MB.") })
                    throw new InvalidOperationException();
                }

                if (string.IsNullOrWhiteSpace(filename))
                {
                    // throw new ValidationErrorException(new[] { new ValidationError(ValidationErrorCodes.EmptyField, $"{nameof(Filename)} cannot be empty.") })
                    throw new InvalidOperationException();
                }

                if (string.IsNullOrWhiteSpace(mimeType))
                {
                    // throw new ValidationErrorException(new[] { new ValidationError(ValidationErrorCodes.EmptyField, $"{nameof(MimeType)} cannot be empty.") })
                    throw new InvalidOperationException();
                }

                if (!mimeType.StartsWith("image/", true, CultureInfo.InvariantCulture))
                {
                    // throw new ValidationErrorException(new[] { new ValidationError(ValidationErrorCodes.InvalidAttachmentType, "Attachments only support images.") })
                    throw new InvalidOperationException();
                }

                Id = id;
                Data = data;
                Filename = filename;
                MimeType = mimeType;
            }

            public int Id { get; }
            public byte[] Data { get; }
            public string Filename { get; }
            public string MimeType { get; }


            public int RequestId { get; set; }

            public IFormFile File { get; set; } = null!;
        }
    }
}