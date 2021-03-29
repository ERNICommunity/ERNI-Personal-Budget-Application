﻿using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Requests;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Business.Commands.Requests
{
    public class SetRequestStateCommand : Command<(int requestId, RequestState requestState)>, ISetRequestStateCommand
    {
        private readonly IMailService _mailService;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public SetRequestStateCommand(
            IMailService mailService,
            IRequestRepository requestRepository,
            IUnitOfWork unitOfWork,
            ILogger<SetRequestStateCommand> logger)
        {
            _mailService = mailService;
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        protected override async Task Execute((int requestId, RequestState requestState) parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(parameter.requestId, cancellationToken);
            if (request == null)
            {
                _logger.LogWarning("Not a valid id");
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Not a valid id");
            }

            if (!Validate(parameter.requestState, request))
            {
                _logger.LogWarning("Validation failed");
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Validation failed");
            }

            request.State = parameter.requestState;

            if (parameter.requestState == RequestState.Completed)
            {
                request.ApprovedDate = DateTime.Now;
            }

            await _unitOfWork.SaveChanges(cancellationToken);

            var message = "Request: " + request.Title + " of amount: " + request.Amount + " has been " +
                          request.State + ".";
            _mailService.SendMail("Your request: " + request.Title + " has been " + request.State + ".", request.User.Username);

            _mailService.SendMail(message, request.User.Username);
        }

        private static bool Validate(RequestState newState, Domain.Models.Entities.Request request) => newState switch
        {
            RequestState.Pending => throw new InvalidOperationException(),
            RequestState.Approved => request.State == RequestState.Pending,
            RequestState.Completed => CanComplete(request),
            RequestState.Rejected => true,
            _ => throw new NotImplementedException(),
        };

        private static bool CanComplete(Domain.Models.Entities.Request request) => request.State == RequestState.Approved;
    }
}