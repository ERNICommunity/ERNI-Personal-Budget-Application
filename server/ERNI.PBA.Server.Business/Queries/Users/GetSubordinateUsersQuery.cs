using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Users;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Responses;

namespace ERNI.PBA.Server.Business.Queries.Users
{
    public class GetSubordinateUsersQuery : Query<IEnumerable<UserModel>>, IGetSubordinateUsersQuery
    {
        private readonly IUserRepository _userRepository;

        public GetSubordinateUsersQuery(IUserRepository userRepository) => _userRepository = userRepository;

        protected override async Task<IEnumerable<UserModel>> Execute(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(principal.GetId(), cancellationToken);
            var users = user.IsAdmin
                ? await _userRepository.GetAllUsers(cancellationToken)
                : await _userRepository.GetSubordinateUsers(user.Id, cancellationToken);

            return users.Select(_ => new UserModel
            {
                Id = _.Id,
                IsAdmin = _.IsAdmin,
                IsSuperior = _.IsSuperior,
                IsViewer = _.IsViewer,
                FirstName = _.FirstName,
                LastName = _.LastName,
                State = _.State,
                Superior = new SuperiorModel
                {
                    Id = _.Superior.Id,
                    FirstName = _.Superior.FirstName,
                    LastName = _.Superior.LastName,
                }
            }).OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName);
        }
    }
}