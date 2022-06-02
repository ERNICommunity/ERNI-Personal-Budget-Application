using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.API;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Responses;

namespace ERNI.PBA.Server.Business.Queries.Budgets
{
    public class GetUsersAvailableForBudgetQuery : Query<BudgetTypeEnum, IEnumerable<UserOutputModel>>, IGetUsersAvailableForBudgetQuery
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;

        public GetUsersAvailableForBudgetQuery(
            IUserRepository userRepository,
            IBudgetRepository budgetRepository)
        {
            _userRepository = userRepository;
            _budgetRepository = budgetRepository;
        }

        protected override async Task<IEnumerable<UserOutputModel>> Execute(BudgetTypeEnum parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            IEnumerable<User> users =
                await _userRepository.GetAllUsers(_ => _.State == UserState.Active, cancellationToken);

            var budgetType = BudgetTypes.Types.Single(_ => _.Id == parameter);

            if (budgetType.SinglePerUser)
            {
                var budgets =
                    (await _budgetRepository.GetBudgetsByYear(DateTime.Now.Year, cancellationToken)).Where(_ =>
                        _.BudgetType == budgetType.Id).Select(_ => _.UserId).ToHashSet();
                users = users.Where(_ => !budgets.Contains(_.Id));
            }

            return users.Select(_ => new UserOutputModel
            {
                Id = _.Id,
                FirstName = _.FirstName,
                LastName = _.LastName
            }).OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName);
        }
    }
}
