using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;

namespace ERNI.PBA.Server.Business.Queries.TeamBudgets
{
    public class GetDefaultTeamBudgetsQuery : Query<int, IEnumerable<GetDefaultTeamBudgetsQuery.TeamBudgetModel>>
    {
        private readonly ITeamBudgetFacade _teamBudgetFacade;
        private readonly IUserRepository _userRepository;

        public GetDefaultTeamBudgetsQuery(ITeamBudgetFacade teamBudgetFacade, IUserRepository userRepository) =>
            (_teamBudgetFacade, _userRepository) = (teamBudgetFacade, userRepository);

        protected override async Task<IEnumerable<TeamBudgetModel>> Execute(int parameter,
            ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(principal.GetId(), cancellationToken);
            var users = await _teamBudgetFacade.GetTeamBudgets(user.Id, parameter, cancellationToken);

            return users.Select(_ => new TeamBudgetModel()
            {
                Employee = new ()
                {
                    Id = _.Employee.Id,
                    FirstName = _.Employee.FirstName,
                    LastName = _.Employee.LastName
                },
                BudgetTotal = _.TotalAmount,
                BudgetLeft = _.TotalAmount - _.SpentAmount
            }).OrderBy(_ => _.Employee.LastName).ThenBy(_ => _.Employee.FirstName);
        }

        public class TeamBudgetModel
        {
            public class EmployeeModel
            {
                public int Id { get; init; }

                public string FirstName { get; init; } = null!;

                public string LastName { get; init; } = null!;
            }

            public EmployeeModel Employee { get; init; } = null!;

            public decimal BudgetTotal { get; init; }

            public decimal BudgetLeft { get; init; }
        }
    }
}