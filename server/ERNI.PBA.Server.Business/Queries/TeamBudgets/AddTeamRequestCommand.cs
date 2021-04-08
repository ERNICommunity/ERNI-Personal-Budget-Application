using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Extensions;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Business.Queries.TeamBudgets
{
    public class CreateTeamRequestCommand : Command<CreateTeamRequestCommand.NewTeamRequestModel>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTeamRequestCommand(
            IUserRepository userRepository,
            IBudgetRepository budgetRepository,
            IRequestRepository requestRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _budgetRepository = budgetRepository;
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Execute(NewTeamRequestModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetId();
            var currentYear = DateTime.Now.Year;

            var budgets =
                await _budgetRepository.GetTeamBudgets(parameter.UserIds, DateTime.Now.Year, cancellationToken);
            var teamBudgets = budgets.ToTeamBudgets();


            var availableFunds = budgets.Sum(_ => _.Amount);
            if (availableFunds < parameter.Amount)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Requested amount {parameter.Amount} exceeds the limit.");
            }

            var user = await _userRepository.GetUser(userId, cancellationToken);

            var transactions = TransactionCalculator.Create(teamBudgets, parameter.Amount);
            var request = new Request
            {
                UserId = user.Id,
                Year = currentYear,
                Title = parameter.Title,
                Amount = parameter.Amount,
                Date = parameter.Date.ToLocalTime(),
                State = RequestState.Pending,
                Transactions = transactions
            };

            await _requestRepository.AddRequest(request);

            await _unitOfWork.SaveChanges(cancellationToken);
        }

        public class NewTeamRequestModel
        {
            public DateTime Date { get; set; }

            public string Title { get; set; } = null!;

            public decimal Amount { get; set; }

            public int[] UserIds { get; set; } = null!;
        }
    }
}
