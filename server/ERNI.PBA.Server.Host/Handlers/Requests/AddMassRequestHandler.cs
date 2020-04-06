using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Commands.Requests;
using ERNI.PBA.Server.Host.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ERNI.PBA.Server.Host.Handlers.Requests
{
    public class AddMassRequestHandler : IRequestHandler<AddMassRequestCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddMassRequestHandler(
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

        public async Task<bool> Handle(AddMassRequestCommand command, CancellationToken cancellationToken)
        {
            var currentUser = await _userRepository.GetUser(command.UserId, cancellationToken);
            if (!currentUser.IsAdmin)
            {
                AppExceptions.AuthorizationException();
            }

            var requests = new List<Request>();
            foreach (var user in command.Users)
            {
                var userId = user.Id;

                var budgets = await _budgetRepository.GetBudgetsByType(user.Id, BudgetTypeEnum.PersonalBudget, command.CurrentYear, cancellationToken);

                if (budgets.Length > 1)
                {
                    throw new OperationErrorException(StatusCodes.Status400BadRequest, $"User {user.Id} has multiple budgets of type {BudgetTypeEnum.PersonalBudget} for year {command.CurrentYear}");
                }

                var budget = budgets.Single();
                if (command.Amount > await GetRemainingAmount(budget, cancellationToken))
                {
                    continue;
                }

                var request = new Request
                {
                    UserId = userId,
                    Year = command.CurrentYear,
                    Title = command.Title,
                    Amount = command.Amount,
                    Date = command.Date.ToLocalTime().Date,
                    CreateDate = DateTime.Now,
                    State = RequestState.Approved,
                    BudgetId = budget.Id
                };

                requests.Add(request);
            }

            await _requestRepository.AddRequests(requests);

            await _unitOfWork.SaveChanges(cancellationToken);

            return true;
        }

        private async Task<decimal> GetRemainingAmount(Budget budget, CancellationToken cancellationToken)
        {
            return budget.Amount - await _budgetRepository.GetTotalRequestedAmount(budget.Id, cancellationToken);
        }
    }
}
