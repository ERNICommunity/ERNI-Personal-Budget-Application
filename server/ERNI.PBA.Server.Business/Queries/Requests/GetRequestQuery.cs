using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Requests;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Business.Queries.Requests
{
    public class GetRequestQuery : Query<int, Request>, IGetRequestQuery
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public GetRequestQuery(
            IRequestRepository requestRepository,
            IUserRepository userRepository,
            ILogger<GetRequestQuery> logger)
        {
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        protected override async Task<Request> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(parameter, cancellationToken);
            if (request == null)
            {
                _logger.LogWarning("Not a valid id");
                throw new OperationErrorException(ErrorCodes.RequestNotFound, "Not a valid id");
            }

            var currentUser = await _userRepository.GetUser(principal.GetId(), cancellationToken)
                              ?? throw AppExceptions.AuthorizationException();

            if (currentUser.Id != request.User.Id && !principal.IsInRole(Roles.Admin) && !principal.IsInRole(Roles.Finance))
            {
                _logger.LogWarning("No access for request!");
                throw AppExceptions.AuthorizationException();
            }

            return new Request
            {
                Id = request.Id,
                Title = request.Title,
                Amount = request.Amount,
                Date = request.Date,
                State = request.State,
                InvoicedAmount = request.InvoicedAmount
            };
        }
    }
}
