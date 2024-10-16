﻿using ERNI.PBA.Server.Business.Infrastructure;
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
using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Business.Queries.InvoiceImages;

public class GetInvoiceImagesQuery(
    IUserRepository userRepository,
    IRequestRepository requestRepository,
    IInvoiceImageRepository invoiceImageRepository) : Query<int, IEnumerable<ImageOutputModel>>, IGetInvoiceImagesQuery
{
    protected override async Task<IEnumerable<ImageOutputModel>> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        var request = await requestRepository.GetRequest(parameter, cancellationToken) ?? throw new OperationErrorException(ErrorCodes.RequestNotFound, "Not a valid id");

        var user = await userRepository.GetUser(principal.GetId(), cancellationToken)
                   ?? throw AppExceptions.AuthorizationException();

        if (!principal.IsInRole(Roles.Admin) && !principal.IsInRole(Roles.Finance) && user.Id != request.UserId &&
            !(request.RequestType == BudgetTypeEnum.CommunityBudget &&
              principal.IsInRole(Roles.CommunityLeader)))
        {
            throw AppExceptions.AuthorizationException();
        }

        var imagesName = await invoiceImageRepository.GetInvoiceImages(parameter, cancellationToken);
        return imagesName.Select(image => new ImageOutputModel
        {
            Id = image.Id,
            Name = image.Name
        });
    }
}