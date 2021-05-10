using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;

namespace ERNI.PBA.Server.Business.Queries.TeamBudgets
{
    public class GetTeamBudgetRequestsQuery : Query<int, IEnumerable<GetTeamBudgetRequestsQuery.TeamRequestModel>>
    {
        private readonly ITeamBudgetFacade _teamBudgetFacade;
        private readonly IUserRepository _userRepository;

        public GetTeamBudgetRequestsQuery(ITeamBudgetFacade teamBudgetFacade, IUserRepository userRepository) =>
            (_teamBudgetFacade, _userRepository) = (teamBudgetFacade, userRepository);

        protected override async Task<IEnumerable<TeamRequestModel>> Execute(int parameter,
            ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(principal.GetId(), cancellationToken);
            var requests = await _teamBudgetFacade.GetTeamRequests(user.Id, parameter, cancellationToken);

            return requests.Select(_ => new TeamRequestModel()
            {
                Transactions = _.Transactions.Select(t =>
                    new TeamRequestModel.TransactionModel
                    {
                        FirstName = t.Budget.User.FirstName,
                        LastName = t.Budget.User.LastName,
                        IsSubordinate = t.Budget.User.SuperiorId == user.Id,
                        Amount = t.Amount,
                        EmployeeId = t.Budget.UserId
                    }
                ).OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToArray(),
                Id = _.Id,
                TotalAmount = _.Amount,
                Title = _.Title,
                State = _.State,
                CreateDate = _.CreateDate
            }).OrderByDescending(_ => _.CreateDate);
        }

        public class TeamRequestModel
        {
            public class TransactionModel
            {
                public int EmployeeId { get; init; }

                public string FirstName { get; init; } = null!;

                public string LastName { get; init; } = null!;

                public decimal Amount { get; init; }

                public bool IsSubordinate { get; init; }
            }

            public int Id { get; init; }

            public TransactionModel[] Transactions { get; init; } = null!;

            public string Title { get; init; } = null!;

            public RequestState State { get; init; }

            public DateTime CreateDate { get; init; }

            public decimal TotalAmount { get; init; }
        }
    }
}