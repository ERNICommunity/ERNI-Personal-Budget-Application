using System;
using System.Globalization;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Security;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Business.Commands.InvoiceImages;

public class AddInvoiceImageCommand(
    IRequestRepository requestRepository,
    IUserRepository userRepository,
    IInvoiceImageRepository invoiceImageRepository,
    IUnitOfWork unitOfWork) : Command<AddInvoiceImageCommand.InvoiceImageModel, int>
{
    protected override async Task<int> Execute(InvoiceImageModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        var requestId = parameter.RequestId;

        var request = await requestRepository.GetRequest(requestId, cancellationToken)
                      ?? throw new OperationErrorException(ErrorCodes.RequestNotFound, "Request not found");

        var user = await userRepository.GetUser(principal.GetId(), cancellationToken);
        if (!principal.IsInRole(Roles.Admin) && (user is null || user.Id != request.UserId))
        {
            throw AppExceptions.AuthorizationException();
        }

        if (request.State != RequestState.Pending)
        {
            throw new OperationErrorException(ErrorCodes.CannotUpdateRequest, "Invoices can be uploaded only to Pending requests.");
        }

        var blobPath = await invoiceImageRepository.UploadImageDataBlob(parameter.Data, request.Id, cancellationToken);

        var image = new InvoiceImage
        {
            Filename = parameter.Filename,
            MimeType = parameter.MimeType,
            BlobPath = blobPath,
            RequestId = requestId
        };

        await invoiceImageRepository.AddInvoiceImage(image, cancellationToken);

        await unitOfWork.SaveChanges(cancellationToken);

        return image.Id;
    }

    public class InvoiceImageModel
    {
        public InvoiceImageModel(int requestId, string? filename, string? mimeType, byte[]? data)
        {
            if (data is null || data.Length == 0)
            {
                // throw new ValidationErrorException(new[] { new ValidationError(ValidationErrorCodes.EmptyField, $"{nameof(Data)} cannot be empty.") })
                throw new InvalidOperationException("Data must not be null");
            }

            var maxSize = 5 * 1024 * 1024;

            if (data.Length > maxSize)
            {
                // throw new ValidationErrorException(new[] { new ValidationError(ValidationErrorCodes.OutOfRange, $"{nameof(Data)} cannot exceed 5 MB.") })
                throw new OperationErrorException(ErrorCodes.MaxSizeExceeded, $"{nameof(Data)} cannot exceed {maxSize} B.");
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                // throw new ValidationErrorException(new[] { new ValidationError(ValidationErrorCodes.EmptyField, $"{nameof(Filename)} cannot be empty.") })
                throw new InvalidOperationException("Filename must not be null");
            }

            if (string.IsNullOrWhiteSpace(mimeType))
            {
                // throw new ValidationErrorException(new[] { new ValidationError(ValidationErrorCodes.EmptyField, $"{nameof(MimeType)} cannot be empty.") })
                throw new InvalidOperationException("MimeType must not be null");
            }

            if (!mimeType.StartsWith("image/", true, CultureInfo.InvariantCulture) && mimeType != "application/pdf")
            {
                // throw new ValidationErrorException(new[] { new ValidationError(ValidationErrorCodes.InvalidAttachmentType, "Attachments only support images.") })
                throw new OperationErrorException(ErrorCodes.InvalidAttachmentType, "Attachments only support images.");
            }

            RequestId = requestId;
            Data = data;
            Filename = filename;
            MimeType = mimeType;
        }

        public int RequestId { get; }
        public byte[] Data { get; }
        public string Filename { get; }
        public string MimeType { get; }
    }
}