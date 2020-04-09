using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Outputs.Budgets;
using ERNI.PBA.Server.Domain.Queries.Budgets;
using ERNI.PBA.Server.Domain.Security;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Business.Handlers.Budgets
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
                throw AppExceptions.AuthorizationException();
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
