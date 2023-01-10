using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;

namespace ERNI.PBA.Server.Business.Queries.Budgets
{
    public class GetBudgetLeftQuery : Query<GetBudgetLeftQuery.UserModel[]>
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

        protected override async Task<UserModel[]> Execute(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var year = DateTime.Now.Year;

            var budgetAmount = (await _budgetRepository.GetTotalAmountsByYear(year, cancellationToken))
                .ToDictionary(_ => _.BudgetId, _ => _.Amount);

            var budgets = await _budgetRepository.GetBudgets(year, BudgetTypeEnum.PersonalBudget, cancellationToken);

            var users = (await _userRepository.GetAllUsers(cancellationToken))
                .ToDictionary(_ => _.Id);

            return budgets
                .Select(budget => (user: users[budget.UserId], amount: budget.Amount - budgetAmount[budget.Id]))
                .Select(tuple => new UserModel(tuple.user.Id, tuple.user.FirstName, tuple.user.LastName, tuple.amount)).ToArray();
        }

        public record UserModel(int Id, string FirstName, string LastName, decimal BudgetLeft);
    }
}