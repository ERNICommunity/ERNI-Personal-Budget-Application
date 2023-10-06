using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Responses.Budgets;
using ERNI.PBA.Server.Domain.Security;

namespace ERNI.PBA.Server.Business.Queries.Budgets
{
    public class GetBudgetQuery : Query<int, SingleBudgetOutputModel>, IGetBudgetQuery
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUserRepository _userRepository;

        public GetBudgetQuery(IBudgetRepository budgetRepository, IUserRepository userRepository)
        {
            _budgetRepository = budgetRepository;
            _userRepository = userRepository;
        }

        protected override async Task<SingleBudgetOutputModel> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(parameter, cancellationToken)
                ?? throw new OperationErrorException(ErrorCodes.BudgetNotFound, $"Budget with id {parameter} not found");

            var user = await _userRepository.GetUser(principal.GetId(), cancellationToken)
                       ?? throw AppExceptions.AuthorizationException();
            if (!principal.IsInRole(Roles.Admin) && user.Id != budget.UserId)
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