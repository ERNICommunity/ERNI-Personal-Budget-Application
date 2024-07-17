using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Export;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Security;

namespace ERNI.PBA.Server.Business.Queries.InvoiceImages
{
    public class GetInvoiceDownloadTokenQuery(IDownloadTokenManager downloadTokenManager, IInvoiceImageRepository invoiceImageRepository, IRequestRepository requestRepository, IUserRepository userRepository) : Query<int, Guid>
    {
        protected override async Task<Guid> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (!principal.IsInRole(Roles.Admin))
            {
                var image = await invoiceImageRepository.GetInvoiceImage(parameter, cancellationToken)
                            ??
                            throw new OperationErrorException(ErrorCodes.InvalidId, "Invoice not found");

                var request = await requestRepository.GetRequest(image.RequestId, cancellationToken)
                              ?? throw new OperationErrorException(ErrorCodes.RequestNotFound, "Request not found");

                var currentUser = await userRepository.GetUser(principal.GetId(), cancellationToken)
                    ?? throw AppExceptions.AuthorizationException();

                if (request.UserId != currentUser.Id && !(request.RequestType == BudgetTypeEnum.CommunityBudget &&
                                                          principal.IsInRole(Roles.CommunityLeader)))
                {
                    throw new OperationErrorException(ErrorCodes.AccessDenied, "Access denied");
                }
            }

            return downloadTokenManager.GenerateToken(DateTime.Now.AddSeconds(10), $"{DownloadTokenCategory.Invoice}-{parameter}");
        }
    }
}