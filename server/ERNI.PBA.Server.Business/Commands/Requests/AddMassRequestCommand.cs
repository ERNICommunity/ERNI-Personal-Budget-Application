using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Requests;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Payloads;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Business.Commands.Requests
{
    public class AddMassRequestCommand : Command<RequestMassModel>, IAddMassRequestCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddMassRequestCommand(
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

        protected override async Task Execute(RequestMassModel parameter, ClaimsPrincipal principal,
            CancellationToken cancellationToken)
        {
            if (!principal.IsInRole(Roles.Admin))
            {
                throw AppExceptions.AuthorizationException();
            }

            var currentYear = DateTime.Now.Year;
            var requests = new List<Request>();
            foreach (var user in parameter.Users)
            {
                var userId = user.Id;

                var budgets = await _budgetRepository.GetBudgetsByType(user.Id, BudgetTypeEnum.PersonalBudget,
                    currentYear, cancellationToken);

                if (budgets.Length > 1)
                {
                    throw new OperationErrorException(StatusCodes.Status400BadRequest,
                        $"User {user.Id} has multiple budgets of type {BudgetTypeEnum.PersonalBudget} for year {currentYear}");
                }

                var budget = budgets.Single();
                if (parameter.Amount > await GetRemainingAmount(budget, cancellationToken))
                {
                    continue;
                }

                var request = new Request
                {
                    UserId = userId,
                    Year = currentYear,
                    Title = parameter.Title,
                    Amount = parameter.Amount,
                    Date = parameter.Date.ToLocalTime().Date,
                    CreateDate = DateTime.Now,
                    State = RequestState.Approved,
                    BudgetId = budget.Id
                };

                requests.Add(request);
            }

            await _requestRepository.AddRequests(requests);

            await _unitOfWork.SaveChanges(cancellationToken);
        }

        private async Task<decimal> GetRemainingAmount(Budget budget, CancellationToken cancellationToken) =>
            budget.Amount - await _budgetRepository.GetTotalRequestedAmount(budget.Id, cancellationToken);
    }
}
