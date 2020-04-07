using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Exceptions;
using ERNI.PBA.Server.Host.Model.Budgets;
using ERNI.PBA.Server.Host.Queries.Budgets;
using ERNI.PBA.Server.Host.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Handlers.Budgets
{
    public class GetBudgetQueryHandler : IRequestHandler<GetBudgetQuery, SingleBudgetOutputModel>
    {
        private readonly IBudgetRepository _budgetRepository;

        public GetBudgetQueryHandler(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public async Task<SingleBudgetOutputModel> Handle(GetBudgetQuery request, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(request.BudgetId, cancellationToken);
            if (budget == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Budget with id {request.BudgetId} not found");
            }

            if (!request.Principal.IsInRole(Roles.Admin) && request.Principal.GetId() != budget.UserId)
            {
                AppExceptions.AuthorizationException();
            }

            return new SingleBudgetOutputModel
            {
                Id = budget.Id,
                Title = budget.Title,
                Year = budget.Year,
                Amount = budget.Amount,
                Type = budget.BudgetType,
                User = new User
                {
                    Id = budget.User.Id,
                    FirstName = budget.User.FirstName,
                    LastName = budget.User.LastName,
                }
            };
        }
    }
}
