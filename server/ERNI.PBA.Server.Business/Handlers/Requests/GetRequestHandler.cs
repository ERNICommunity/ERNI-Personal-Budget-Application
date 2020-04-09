using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Queries.Requests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Business.Handlers.Requests
{
    public class GetRequestHandler : IRequestHandler<GetRequestQuery, Request>
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public GetRequestHandler(
            IRequestRepository requestRepository,
            IUserRepository userRepository,
            ILogger<GetRequestHandler> logger)
        {
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Request> Handle(GetRequestQuery command, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(command.RequestId, cancellationToken);
            if (request == null)
            {
                _logger.LogWarning("Not a valid id");
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Not a valid id");
            }

            var currentUser = await _userRepository.GetUser(command.Principal.GetId(), cancellationToken);
            var isAdmin = currentUser.IsAdmin;
            var isViewer = currentUser.IsViewer;

            if (currentUser.Id != request.User.Id && !isAdmin && !isViewer)
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
            };
        }
    }
}
