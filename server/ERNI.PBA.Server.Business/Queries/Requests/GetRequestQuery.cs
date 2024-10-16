﻿using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Commands.Users;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Requests;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Business.Queries.Requests
{
    public class GetRequestQuery(
        IRequestRepository requestRepository,
        IUserRepository userRepository,
        ILogger<GetRequestQuery> logger) : Query<int, Request>, IGetRequestQuery
    {
        private static readonly Action<ILogger, int, int, Exception?> _userNotFound = LoggerMessage.Define<int, int>(
            LogLevel.Information,
            new EventId(1, nameof(UpdateUserCommand)),
            "Unauthorized access for {UserId} to request {RequestId}");
        private readonly ILogger _logger = logger;

        protected override async Task<Request> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var request = await requestRepository.GetRequest(parameter, cancellationToken);
            if (request == null)
            {
                _logger.RequestNotFound(parameter);

                throw new OperationErrorException(ErrorCodes.RequestNotFound, "Not a valid id");
            }

            var currentUser = await userRepository.GetUser(principal.GetId(), cancellationToken)
                              ?? throw AppExceptions.AuthorizationException();

            if (currentUser.Id != request.User.Id && !principal.IsInRole(Roles.Admin) &&
                !principal.IsInRole(Roles.Finance) && !(request.RequestType == BudgetTypeEnum.CommunityBudget &&
                                                        principal.IsInRole(Roles.CommunityLeader)))
            {
                _userNotFound(_logger, currentUser.Id, parameter, null);
                throw AppExceptions.AuthorizationException();
            }

            return new Request
            {
                Id = request.Id,
                Title = request.Title,
                Amount = request.Amount,
                Date = request.Date,
                State = request.State,
            };
        }
    }
}
