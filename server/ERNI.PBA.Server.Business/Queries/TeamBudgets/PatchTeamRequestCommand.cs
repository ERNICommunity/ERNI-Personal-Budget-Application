using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Security;

namespace ERNI.PBA.Server.Business.Queries.TeamBudgets
{
    public class PatchTeamRequestCommand : Command<(int RequestId, PatchTeamRequestCommand.PatchTeamRequestModel Payload)>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITeamBudgetFacade _teamBudgetFacade;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PatchTeamRequestCommand(
            IUserRepository userRepository,
            ITeamBudgetFacade teamBudgetFacade,
            IRequestRepository requestRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _teamBudgetFacade = teamBudgetFacade;
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Execute((int RequestId, PatchTeamRequestModel Payload) parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetId();

            var user = await _userRepository.GetUser(userId, cancellationToken);

            var request = await _requestRepository.GetRequest(parameter.RequestId, cancellationToken);

            if (!principal.IsInRole(Roles.Admin))
            {
                throw new OperationErrorException(ErrorCodes.AccessDenied, "Access denied");
            }

            if (!principal.IsInRole(Roles.Admin) &&
                (request.State == RequestState.Completed || request.State == RequestState.Rejected))
            {
                throw new OperationErrorException(ErrorCodes.ValidationError,
                    $"Cannot update completed or rejected requests.");
            }

            var allBudgets =
                await _teamBudgetFacade.GetTeamBudgets(user.Id, DateTime.Now.Year, cancellationToken);
            var dict = allBudgets.ToDictionary(_ => _.Employee.Id);
            var unknownUsers = parameter.Payload.Employees.Where(id => !dict.ContainsKey(id)).ToList();

            if (unknownUsers.Any())
            {
                throw new OperationErrorException(ErrorCodes.ValidationError, $"Employees not found: {string.Join(",", unknownUsers)}");
            }

            var teamBudgets = parameter.Payload.Employees.Select(_ => dict[_])
                .Select(_ => new TeamBudget() { BudgetId = _.BudgetId, Amount = _.SpentAmount, UserId = _.Employee.Id });

            if (parameter.Payload.Amount <= 0.0m)
            {
                throw new OperationErrorException(ErrorCodes.InvalidAmount, $"The requested amount ({parameter.Payload.Amount}) has to be positive.");
            }

            var availableFunds = teamBudgets.Sum(_ => _.Amount);
            if (availableFunds < parameter.Payload.Amount)
            {
                throw new OperationErrorException(ErrorCodes.InvalidAmount, $"The requested amount {parameter.Payload.Amount} exceeds the limit.");
            }

            var transactions = TransactionCalculator.Create(teamBudgets, parameter.Payload.Amount);

            request.Title = parameter.Payload.Title;
            request.Amount = parameter.Payload.Amount;
            request.InvoicedAmount = parameter.Payload.Amount;
            request.Date = parameter.Payload.Date.ToLocalTime();

            await _requestRepository.AddOrUpdateTransactions(parameter.RequestId, transactions);

            await _unitOfWork.SaveChanges(cancellationToken);
        }

        public class PatchTeamRequestModel
        {
            public DateTime Date { get; set; }

            public string Title { get; set; } = null!;

            public decimal Amount { get; set; }

            public int[] Employees { get; set; } = null!;
        }
    }
}