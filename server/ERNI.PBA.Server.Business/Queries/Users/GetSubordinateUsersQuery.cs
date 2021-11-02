using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Security;

namespace ERNI.PBA.Server.Business.Queries.Users
{
    public class GetSubordinateUsersQuery : Query<IEnumerable<GetSubordinateUsersQuery.UserModel>>
    {
        private readonly IUserRepository _userRepository;

        public GetSubordinateUsersQuery(IUserRepository userRepository) => _userRepository = userRepository;

        protected override async Task<IEnumerable<UserModel>> Execute(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(principal.GetId(), cancellationToken);
            var users = principal.IsInRole(Roles.Admin)
                ? await _userRepository.GetAllUsers(cancellationToken)
                : await _userRepository.GetSubordinateUsers(user.Id, cancellationToken);

            return users.Select(_ => new UserModel
            {
                Id = _.Id,
                FirstName = _.FirstName,
                LastName = _.LastName,
                State = _.State,
                Email = _.Username,
                Superior = user.Superior is not null
                    ? new SuperiorModel
                    {
                        Id = user.Superior.Id,
                        FirstName = user.Superior.FirstName,
                        LastName = user.Superior.LastName,
                    }
                    : null
            }).OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName);
        }

        public class UserModel
        {
            public int Id { get; init; }

            public string FirstName { get; init; } = null!;

            public string LastName { get; init; } = null!;

            public string Email { get; init; } = null!;

            public SuperiorModel? Superior { get; init; }

            public UserState State { get; init; }
        }

        public class SuperiorModel
        {
            public int Id { get; init; }

            public string FirstName { get; init; } = null!;

            public string LastName { get; init; } = null!;
        }
    }
}