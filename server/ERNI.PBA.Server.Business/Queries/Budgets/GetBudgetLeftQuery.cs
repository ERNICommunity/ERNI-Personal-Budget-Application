using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Payloads;
using ERNI.PBA.Server.Domain.Models.Responses;

namespace ERNI.PBA.Server.Business.Queries.Budgets
{
    public class GetBudgetLeftQuery : Query<BudgetLeftModel, UserModel[]>, IGetBudgetLeftQuery
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUserRepository _userRepository;

        public GetBudgetLeftQuery(
            IBudgetRepository budgetRepository,
            IUserRepository userRepository)
        {
            _budgetRepository = budgetRepository;
            _userRepository = userRepository;
        }

        protected override async Task<UserModel[]> Execute(BudgetLeftModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var budgetAmount = (await _budgetRepository.GetTotalAmountsByYear(parameter.Year, cancellationToken))
                .ToDictionary(_ => _.BudgetId, _ => _.Amount);

            var budgets = await _budgetRepository.GetBudgets(parameter.Year, BudgetTypeEnum.PersonalBudget, cancellationToken);

            var users = (await _userRepository.GetAllUsers(cancellationToken))
                .ToDictionary(_ => _.Id);

            return budgets
                .Where(budget => budgetAmount[budget.Id] + parameter.Amount <= budget.Amount)
                .Select(budget => users[budget.UserId])
                .Select(user => new UserModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    State = user.State,
                    Superior = user.Superior is not null
                        ? new SuperiorModel
                        {
                            Id = user.Superior.Id,
                            FirstName = user.Superior.FirstName,
                            LastName = user.Superior.LastName,
                        }
                        : null
                }).ToArray();
        }
    }
}