using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Exceptions;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Model.PendingRequests;
using ERNI.PBA.Server.Host.Queries.Budgets;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Handlers.Budgets
{
    public class GetTeamBudgetByYearHandler : IRequestHandler<GetBudgetByYearQuery, BudgetOutputModel[]>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;

        public GetTeamBudgetByYearHandler(
            IUserRepository userRepository,
            IBudgetRepository budgetRepository)
        {
            _userRepository = userRepository;
            _budgetRepository = budgetRepository;
        }

        public async Task<BudgetOutputModel[]> Handle(GetBudgetByYearQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(request.UserId, cancellationToken);
            if (!user.IsSuperior)
            {
                throw AppExceptions.AuthorizationException();
            }

            var budgets = await _budgetRepository.GetTeamBudgets(request.UserId, request.Year, cancellationToken);
            if (!budgets.Any())
            {
                return Array.Empty<BudgetOutputModel>();
            }

            var masterBudget = budgets.SingleOrDefault(x => x.UserId == request.UserId);
            if (masterBudget == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Cumulative budget does not exists");
            }

            var amount = budgets.Sum(_ => _.Amount);
            var amountLeft = amount - budgets.SelectMany(_ => _.Transactions.Where(x => x.Request.State != RequestState.Rejected)).Sum(_ => _.Amount);

            var model = new BudgetOutputModel
            {
                Id = masterBudget.Id,
                Year = masterBudget.Year,
                Amount = amount,
                AmountLeft = amountLeft,
                Title = masterBudget.Title,
                Type = masterBudget.BudgetType,
                Requests = masterBudget.Requests.Select(_ => new RequestOutputModel
                {
                    Id = _.Id,
                    Title = _.Title,
                    Amount = _.Amount,
                    Date = _.Date,
                    CreateDate = _.CreateDate,
                    State = _.State
                })
            };

            return new[] { model };
        }
    }
}
