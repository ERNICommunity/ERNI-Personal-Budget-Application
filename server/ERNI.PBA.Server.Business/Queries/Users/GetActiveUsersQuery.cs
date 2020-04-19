using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Users;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Responses;

namespace ERNI.PBA.Server.Business.Queries.Users
{
    public class GetActiveUsersQuery : Query<IEnumerable<UserModel>>, IGetActiveUsersQuery
    {
        private readonly IUserRepository _userRepository;

        public GetActiveUsersQuery(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected override async Task<IEnumerable<UserModel>> Execute(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllUsers(_ => _.State == UserState.Active, cancellationToken);

            return users.Select(_ => new UserModel
            {
                Id = _.Id,
                FirstName = _.FirstName,
                LastName = _.LastName,
            }).OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName);
        }
    }
}
